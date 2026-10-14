using System.Net;
using System.Net.Http.Json;
using PAW.Models.DTO;
using PawTask = PAW.Models.Task;

namespace PAW.Web.Services;

public interface ITaskService
{
    Task<IEnumerable<PawTaskDTO>> GetTasksAsync();
    Task<PawTaskDTO?> GetTaskByIdAsync(int id);
    Task<bool> SaveTaskAsync(PawTask task);
    Task<bool> DeleteTaskAsync(int id);
}

public class TaskService(HttpClient client) : ITaskService
{
    public async Task<IEnumerable<PawTaskDTO>> GetTasksAsync() =>
        await client.GetFromJsonAsync<List<PawTaskDTO>>("tasks") ?? [];

    public async Task<PawTaskDTO?> GetTaskByIdAsync(int id)
    {
        using var response = await client.GetAsync($"tasks/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PawTaskDTO>();
    }

    public async Task<bool> SaveTaskAsync(PawTask task)
    {
        var dto = PawTaskDTO.ConvertFrom(task);
        using var response = task.Id == 0
            ? await client.PostAsJsonAsync("tasks", dto)
            : await client.PutAsJsonAsync($"tasks/{task.Id}", dto);
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        using var response = await client.DeleteAsync($"tasks/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }
}