using System.Text.Json;
using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface IInventoryService
{
    Task<IEnumerable<InventoryDTO>> GetInventoriesAsync();

    Task<InventoryDTO?> GetInventoryByIdAsync(int id);

    Task<bool> CreateInventoryAsync(Inventory inventory);

    Task<bool> SaveInventoryAsync(Inventory inventory);

    Task<bool> DeleteInventoryAsync(int id);
}

public class InventoryService : ServiceBase, IInventoryService
{
    private const string _path = "Inventory";

    private readonly IRestProvider _restProvider;

    public InventoryService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<IEnumerable<InventoryDTO>> GetInventoriesAsync()
    {
        var response =
            await _restProvider.GetAsync(
                SetPathUrl(_path),
                id: null);

        return await JsonProvider
            .DeserializeAsync<IEnumerable<InventoryDTO>>(response);
    }

    public async Task<InventoryDTO?> GetInventoryByIdAsync(int id)
    {
        var response =
            await _restProvider.GetAsync(
                SetPathUrl(_path),
                id.ToString());

        return await JsonProvider
            .DeserializeAsync<InventoryDTO>(response);
    }

    public async Task<bool> CreateInventoryAsync(
        Inventory inventory)
    {
        var json = JsonSerializer.Serialize(inventory);

        var response =
            await _restProvider.PostAsync(
                SetPathUrl(_path) + "/create",
                json);

        return JsonSerializer.Deserialize<bool>(response);
    }

    public async Task<bool> SaveInventoryAsync(
        Inventory inventory)
    {
        var json =
            JsonSerializer.Serialize(new[] { inventory });

        var response =
            await _restProvider.PostAsync(
                SetPathUrl(_path),
                json);

        return JsonSerializer.Deserialize<bool>(response);
    }

    public async Task<bool> DeleteInventoryAsync(int id)
    {
        var response =
            await _restProvider.DeleteAsync(
                SetPathUrl(_path),
                id.ToString());

        return JsonSerializer.Deserialize<bool>(response);
    }
}