using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;
using PawTask = PAW.Models.Task;

namespace PAW.Web.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(
            ITaskService taskService,
            ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var tasks =
                await _taskService.GetTasksAsync();

            const int pageSize = 25;

            var totalItems = tasks.Count();

            if (page < 1)
                page = 1;

            var totalPages =
                (int)Math.Ceiling((double)totalItems / pageSize);

            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            var items = tasks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedResult<PawTaskDTO>
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
        public async Task<IActionResult> Create(PawTask task)
        {
            if (!ModelState.IsValid)
                return View(task);

            await _taskService
                .SaveTaskAsync(task);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var task =
                await _taskService.GetTaskByIdAsync(id);

            if (task == null)
                return NotFound();

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            [FromForm] PawTaskDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var task = PawTaskDTO.ConvertTo(dto);

            await _taskService
                .SaveTaskAsync(task);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var task =
                await _taskService.GetTaskByIdAsync(id);

            if (task == null)
                return NotFound();

            return PartialView(
                "_DetailsPartial",
                task);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _taskService
                .DeleteTaskAsync(id);

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