using System.Text.Json;
using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface IUserRoleService
{
    Task<IEnumerable<UserRoleDTO>> GetUserRolesAsync();
    Task<UserRoleDTO?> GetUserRoleByIdAsync(int id);
    Task<bool> CreateUserRoleAsync(UserRole userRole);
    Task<bool> SaveUserRoleAsync(UserRole userRole);
    Task<bool> DeleteUserRoleAsync(int id);
}

public class UserRoleService : ServiceBase, IUserRoleService
{
    private const string _path = "UserRole";
    private readonly IRestProvider _restProvider;

    public UserRoleService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<IEnumerable<UserRoleDTO>> GetUserRolesAsync()
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id: null);
        var userRoles = await JsonProvider.DeserializeAsync<IEnumerable<UserRoleDTO>>(response);
        return userRoles;
    }

    public async Task<UserRoleDTO?> GetUserRoleByIdAsync(int id)
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id.ToString());
        return await JsonProvider.DeserializeAsync<UserRoleDTO>(response);
    }

    public async Task<bool> CreateUserRoleAsync(UserRole userRole)
    {
        var json = JsonSerializer.Serialize(userRole);

        var response = await _restProvider.PostAsync(SetPathUrl(_path) + "/create", json);

        return JsonSerializer.Deserialize<bool>(response);
    }

    public async Task<bool> SaveUserRoleAsync(UserRole userRole)
    {
        var json = JsonSerializer.Serialize(new[] { userRole });

        var response = await _restProvider.PostAsync(SetPathUrl(_path), json);

        return JsonSerializer.Deserialize<bool>(response);
    }

    public async Task<bool> DeleteUserRoleAsync(int id)
    {
        var response = await _restProvider.DeleteAsync(SetPathUrl(_path), id.ToString());

        return JsonSerializer.Deserialize<bool>(response);
    }
}