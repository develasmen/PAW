using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAW.Models;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            INotificationService notificationService,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var notifications =
                await _notificationService.GetNotificationsAsync();

            const int pageSize = 25;

            var totalItems = notifications.Count();

            if (page < 1)
                page = 1;

            var totalPages =
                (int)Math.Ceiling((double)totalItems / pageSize);

            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            var items = notifications
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedResult<NotificationDTO>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Notification notification)
        {
            if (!ModelState.IsValid)
                return View(notification);

            await _notificationService
                .SaveNotificationAsync(notification);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var notification =
                await _notificationService.GetNotificationByIdAsync(id);

            if (notification == null)
                return NotFound();

            return View(notification);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            [FromForm] NotificationDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var notification = NotificationDTO.ConvertTo(dto);

            await _notificationService
                .SaveNotificationAsync(notification);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var notification =
                await _notificationService.GetNotificationByIdAsync(id);

            if (notification == null)
                return NotFound();

            return PartialView(
                "_DetailsPartial",
                notification);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _notificationService
                .DeleteNotificationAsync(id);

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id ??
                    HttpContext.TraceIdentifier
            });
        }
    }
}