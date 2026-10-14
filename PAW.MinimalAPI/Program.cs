using System.ComponentModel.DataAnnotations;
using PAW.MinimalAPI.Repositories;
using PAW.Models.DTO;
using PawTask = PAW.Models.Task;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ITasksRepository, SqlTasksRepository>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();
app.UseExceptionHandler();

// Los handlers reciben el repositorio del contenedor DI/IoC.
var tasks = app.MapGroup("/tasks").WithTags("Tasks");
tasks.MapGet("/", async (ITasksRepository repository, CancellationToken ct) =>
    Results.Ok((await repository.GetAllAsync(ct)).Select(PawTaskDTO.ConvertFrom)));
tasks.MapGet("/{id:int}", async (int id, ITasksRepository repository, CancellationToken ct) =>
    await repository.GetByIdAsync(id, ct) is { } task
        ? Results.Ok(PawTaskDTO.ConvertFrom(task)) : Results.NotFound());

tasks.MapPost("/", async (PawTaskDTO dto, ITasksRepository repository, CancellationToken ct) =>
{
    if (Validate(dto) is { } errors) return Results.ValidationProblem(errors);
    if (dto.Id != 0) return Results.BadRequest(new { message = "El ID se genera al crear la tarea." });
    var task = await repository.CreateAsync(PawTaskDTO.ConvertTo(dto), ct);
    return Results.Created($"/tasks/{task.Id}", PawTaskDTO.ConvertFrom(task));
});
tasks.MapPut("/{id:int}", async (int id, PawTaskDTO dto, ITasksRepository repository, CancellationToken ct) =>
{
    if (Validate(dto) is { } errors) return Results.ValidationProblem(errors);
    if (dto.Id != 0 && dto.Id != id) return Results.BadRequest(new { message = "El ID no coincide con la ruta." });
    return await repository.UpdateAsync(id, PawTaskDTO.ConvertTo(dto), ct) is { } task
        ? Results.Ok(PawTaskDTO.ConvertFrom(task)) : Results.NotFound();
});
tasks.MapDelete("/{id:int}", async (int id, ITasksRepository repository, CancellationToken ct) =>
    await repository.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
app.Run();

static Dictionary<string, string[]>? Validate(PawTaskDTO dto)
{
    var results = new List<ValidationResult>();
    Validator.TryValidateObject(dto, new ValidationContext(dto), results, true);
    if (dto.DueDate is { } date && date < new DateTime(1753, 1, 1))
        results.Add(new ValidationResult("La fecha debe ser posterior a 1753-01-01.", [nameof(dto.DueDate)]));
    return results.Count == 0 ? null : results.SelectMany(result => result.MemberNames.Select(member =>
            new { member, message = result.ErrorMessage ?? "Valor inválido." }))
        .GroupBy(result => result.member).ToDictionary(group => group.Key, group => group.Select(result => result.message).ToArray());
}