using System.Text.Json;
using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.Web.Services;

public interface INotificationService
{
    Task<IEnumerable<NotificationDTO>> GetNotificationsAsync();
    Task<NotificationDTO?> GetNotificationByIdAsync(int id);
    Task<bool> SaveNotificationAsync(Notification notification);
    Task<bool> DeleteNotificationAsync(int id);
}

public class NotificationService : ServiceBase, INotificationService
{
    private const string _path = "Notification";
    private readonly IRestProvider _restProvider;

    public NotificationService(IRestProvider restProvider)
    {
        _restProvider = restProvider;
    }

    public async Task<IEnumerable<NotificationDTO>> GetNotificationsAsync()
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id: null);
        var notifications = await JsonProvider.DeserializeAsync<IEnumerable<NotificationDTO>>(response);
        return notifications;
    }

    public async Task<NotificationDTO?> GetNotificationByIdAsync(int id)
    {
        var response = await _restProvider.GetAsync(SetPathUrl(_path), id.ToString());
        return await JsonProvider.DeserializeAsync<NotificationDTO>(response);
    }

    public async Task<bool> SaveNotificationAsync(Notification notification)
    {
        var json = JsonSerializer.Serialize(new[] { notification });

        var response = await _restProvider.PostAsync(
            SetPathUrl(_path),
            json);

        return JsonSerializer.Deserialize<bool>(response);
    }

    public async Task<bool> DeleteNotificationAsync(int id)
    {
        var response = await _restProvider.DeleteAsync(
            SetPathUrl(_path),
            id.ToString());

        return JsonSerializer.Deserialize<bool>(response);
    }
}
