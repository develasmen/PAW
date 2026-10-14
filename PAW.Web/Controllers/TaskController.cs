using Microsoft.AspNetCore.Mvc;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;
using PawTask = PAW.Models.Task;

namespace PAW.Web.Controllers;

public class TaskController(ITaskService taskService, ILogger<TaskController> logger) : Controller
{
    public async Task<IActionResult> Index(int page = 1)
    {
        List<PawTaskDTO> tasks;
        try { tasks = (await taskService.GetTasksAsync()).ToList(); }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogError(ex, "No se pudieron cargar las tareas.");
            ViewData["TasksError"] = "No se pudieron cargar las tareas. Verifique que PAW.MinimalAPI esté en ejecución y que la base de datos esté disponible.";
            tasks = [];
        }
        const int pageSize = 25;
        var totalPages = Math.Max(1, (int)Math.Ceiling((double)tasks.Count / pageSize));
        page = Math.Clamp(page, 1, totalPages);
        return View(new PagedResult<PawTaskDTO>
        {
            Items = tasks.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
            CurrentPage = page, PageSize = pageSize, TotalItems = tasks.Count
        });
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PawTask task)
    {
        task.Id = 0;
        if (!ModelState.IsValid) return View(task);
        try { await taskService.SaveTaskAsync(task); }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogError(ex, "No se pudo crear la tarea.");
            ModelState.AddModelError(string.Empty, "No se pudo guardar la tarea. Revise los datos y la conexión con la API.");
            return View(task);
        }
        TempData["TasksSuccess"] = "Tarea creada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var task = await taskService.GetTaskByIdAsync(id);
            return task is null ? NotFound() : View(task);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogError(ex, "No se pudo consultar la tarea.");
            return StatusCode(503, "No se pudo consultar la tarea. La API no está disponible.");
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromForm] PawTaskDTO dto)
    {
        if (dto.Id <= 0) return BadRequest();
        if (!ModelState.IsValid) return View(dto);
        try { await taskService.SaveTaskAsync(PawTaskDTO.ConvertTo(dto)); }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogError(ex, "No se pudo actualizar la tarea.");
            ModelState.AddModelError(string.Empty, "No se pudo guardar la tarea. Revise los datos y la conexión con la API.");
            return View(dto);
        }
        TempData["TasksSuccess"] = "Tarea actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var task = await taskService.GetTaskByIdAsync(id);
            return task is null ? NotFound() : PartialView("_DetailsPartial", task);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogError(ex, "No se pudieron consultar los detalles.");
            return StatusCode(503, "No se pudieron consultar los detalles de la tarea.");
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (!await taskService.DeleteTaskAsync(id)) return NotFound();
            TempData["TasksSuccess"] = "Tarea eliminada correctamente.";
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogError(ex, "No se pudo eliminar la tarea.");
            TempData["TasksError"] = "No se pudo eliminar la tarea. Intente nuevamente.";
        }
        return RedirectToAction(nameof(Index));
    }
}