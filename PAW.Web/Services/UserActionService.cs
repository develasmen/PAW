using System.Text.Json;
using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface IUserActionService
{
    Task<IEnumerable<UserActionDTO>> GetUserActionsAsync();
    Task<UserActionDTO?> GetUserActionByIdAsync(int id);
    Task<bool> SaveUserActionAsync(UserAction userAction);
    Task<bool> CreateUserActionAsync(UserAction userAction);
    Task<bool> DeleteUserActionAsync(int id);
}

public class UserActionService : ServiceBase, IUserActionService
{
    private const string _path = "UserAction";
    private readonly IRestProvider _restProvider;

    public UserActionService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<IEnumerable<UserActionDTO>> GetUserActionsAsync()
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id: null);
        var userActions = await JsonProvider.DeserializeAsync<IEnumerable<UserActionDTO>>(response);
        return userActions;
    }

    public async Task<UserActionDTO?> GetUserActionByIdAsync(int id)
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id.ToString());
        return await JsonProvider.DeserializeAsync<UserActionDTO>(response);
    }

    public async Task<bool> SaveUserActionAsync(UserAction userAction)
    {
        var json = JsonSerializer.Serialize(new[] { userAction });

        var response = await _restProvider.PostAsync(SetPathUrl(_path), json);

        return JsonSerializer.Deserialize<bool>(response);
    }

    public async Task<bool> CreateUserActionAsync(UserAction userAction)
    {
        var json = JsonSerializer.Serialize(userAction);

        var response = await _restProvider.PostAsync(SetPathUrl(_path) + "/create", json);

        return JsonSerializer.Deserialize<bool>(response);
    }

    public async Task<bool> DeleteUserActionAsync(int id)
    {
        var response = await _restProvider.DeleteAsync(SetPathUrl(_path), id.ToString());

        return JsonSerializer.Deserialize<bool>(response);
    }

}