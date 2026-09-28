using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface IUserRoleService
{
    Task<IEnumerable<UserRoleDTO>> GetUserRolesAsync();
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
}