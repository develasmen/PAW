using APW.Architecture;
using System.Text.Json;
using PAW.Architecture.Providers;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetUsersAsync();
    Task<UserDTO?> GetUserByIdAsync(int id);
    Task<bool> SaveUserAsync(UserDTO user);
    Task<bool> DeleteUserAsync(int id);
}

public class UserService : ServiceBase, IUserService
{
    private const string Path = "User";
    private readonly IRestProvider _restProvider;

    public UserService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<IEnumerable<UserDTO>> GetUsersAsync()
    {
        var response = await _restProvider.GetAsync(SetPathUrl(Path), id: null);
        return await JsonProvider.DeserializeAsync<IEnumerable<UserDTO>>(response);
    }

    public async Task<UserDTO?> GetUserByIdAsync(int id)
    {
        var response = await _restProvider.GetAsync(SetPathUrl(Path), id.ToString());
        return await JsonProvider.DeserializeAsync<UserDTO>(response);
    }

    public async Task<bool> SaveUserAsync(UserDTO user)
    {
        var response = await _restProvider.PostAsync(
            SetPathUrl(Path), JsonSerializer.Serialize(new[] { user }));
        return JsonSerializer.Deserialize<bool>(response);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var response = await _restProvider.DeleteAsync(SetPathUrl(Path), id.ToString());
        return JsonSerializer.Deserialize<bool>(response);
    }
}
