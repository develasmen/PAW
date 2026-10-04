using System.Text.Json;
using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface IComponentService
{
    Task<IEnumerable<ComponentDTO>> GetComponentsAsync();

    Task<ComponentDTO?> GetComponentByIdAsync(int id);

    Task<bool> CreateComponentAsync(Component component);

    Task<bool> SaveComponentAsync(Component component);

    Task<bool> DeleteComponentAsync(int id);
}

public class ComponentService : ServiceBase, IComponentService
{
    private const string _path = "Component";

    private readonly IRestProvider _restProvider;

    public ComponentService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<IEnumerable<ComponentDTO>> GetComponentsAsync()
    {
        var response =
            await _restProvider.GetAsync(SetPathUrl(_path), id: null);

        return await JsonProvider
            .DeserializeAsync<IEnumerable<ComponentDTO>>(response);
    }

    public async Task<ComponentDTO?> GetComponentByIdAsync(int id)
    {
        var response =
            await _restProvider.GetAsync(
                SetPathUrl(_path),
                id.ToString());

        return await JsonProvider
            .DeserializeAsync<ComponentDTO>(response);
    }

    public async Task<bool> CreateComponentAsync(Component component)
    {
        var json = JsonSerializer.Serialize(component);

        var response =
            await _restProvider.PostAsync(
                SetPathUrl(_path) + "/create",
                json);

        return JsonSerializer.Deserialize<bool>(response);
    }

    public async Task<bool> SaveComponentAsync(Component component)
    {
        var json = JsonSerializer.Serialize(new[] { component });

        var response =
            await _restProvider.PostAsync(
                SetPathUrl(_path),
                json);

        return JsonSerializer.Deserialize<bool>(response);
    }

    public async Task<bool> DeleteComponentAsync(int id)
    {
        var response =
            await _restProvider.DeleteAsync(
                SetPathUrl(_path),
                id.ToString());

        return JsonSerializer.Deserialize<bool>(response);
    }
}