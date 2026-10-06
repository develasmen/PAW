using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController(
        ILogger<NotificationController> logger,
        INotificationRepository notificationRepository) : ControllerBase
    {
        [HttpGet(Name = "GetNotifications")]
        public async Task<IEnumerable<NotificationDTO>> GetAll()
        {
            var notifications = await notificationRepository.ReadAsync() ?? [];

            return notifications.Select(NotificationDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetNotificationById")]
        public async Task<ActionResult<NotificationDTO>> GetById(int id)
        {
            var notification = await notificationRepository.FindAsync(id);

            if (notification == null)
            {
                return NotFound();
            }

            return NotificationDTO.ConvertFrom(notification);
        }

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                if (notification.Id > 0)
                    await notificationRepository.UpdateAsync(notification);
                else
                    await notificationRepository.CreateAsync(notification);
            }

            return true;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var notification = await notificationRepository.FindAsync(id);

            if (notification == null)
                return NotFound();

            var result = await notificationRepository.DeleteAsync(notification);

            return Ok(result);
        }
    }
}
