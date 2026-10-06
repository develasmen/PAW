using APW.Architecture;
using System.Text.Json;
using PAW.Architecture.Providers;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDTO>> GetRolesAsync();
    Task<RoleDTO?> GetRoleByIdAsync(int id);
    Task<bool> SaveRoleAsync(Role role);
    Task<bool> DeleteRoleAsync(int id);
}

public class RoleService : ServiceBase, IRoleService
{
    private const string Path = "Role";
    private readonly IRestProvider _restProvider;

    public RoleService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<IEnumerable<RoleDTO>> GetRolesAsync()
    {
        var response = await _restProvider.GetAsync(SetPathUrl(Path), id: null);
        return await JsonProvider.DeserializeAsync<IEnumerable<RoleDTO>>(response);
    }

    public async Task<RoleDTO?> GetRoleByIdAsync(int id)
    {
        var response = await _restProvider.GetAsync(SetPathUrl(Path), id.ToString());
        return await JsonProvider.DeserializeAsync<RoleDTO>(response);
    }

    public async Task<bool> SaveRoleAsync(Role role)
    {
        var response = await _restProvider.PostAsync(
            SetPathUrl(Path), JsonSerializer.Serialize(new[] { role }));
        return JsonSerializer.Deserialize<bool>(response);
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        var response = await _restProvider.DeleteAsync(SetPathUrl(Path), id.ToString());
        return JsonSerializer.Deserialize<bool>(response);
    }
}
