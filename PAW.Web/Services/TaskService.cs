using System.Text.Json;
using APW.Architecture;
using PAW.Architecture.Providers;
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

public class TaskService : ServiceBase, ITaskService
{
    private const string _path = "Task";
    private readonly IRestProvider _restProvider;

    public TaskService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<IEnumerable<PawTaskDTO>> GetTasksAsync()
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id: null);
        var tasks = await JsonProvider.DeserializeAsync<IEnumerable<PawTaskDTO>>(response);
        return tasks;
    }

    public async Task<PawTaskDTO?> GetTaskByIdAsync(int id)
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id.ToString());
        return await JsonProvider.DeserializeAsync<PawTaskDTO>(response);
    }

    public async Task<bool> SaveTaskAsync(PawTask task)
    {
        var json = JsonSerializer.Serialize(new[] { task });

        var response = await _restProvider.PostAsync(
            SetPathUrl(_path),
            json);

        return JsonSerializer.Deserialize<bool>(response);
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var response = await _restProvider.DeleteAsync(
            SetPathUrl(_path),
            id.ToString());

        return JsonSerializer.Deserialize<bool>(response);
    }
}