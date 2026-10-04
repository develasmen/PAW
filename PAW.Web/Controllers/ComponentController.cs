using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAW.Models;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    public class ComponentController : Controller
    {
        private readonly IComponentService _componentService;
        private readonly ILogger<ComponentController> _logger;

        public ComponentController(
            IComponentService componentService,
            ILogger<ComponentController> logger)
        {
            _componentService = componentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var components =
                await _componentService.GetComponentsAsync();

            const int pageSize = 25;

            var totalItems = components.Count();

            if (page < 1)
                page = 1;

            var totalPages = (int)Math.Ceiling(
                (double)totalItems / pageSize);

            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            var items = components
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedResult<ComponentDTO>
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
        public async Task<IActionResult> Create(Component component)
        {
            if (!ModelState.IsValid)
                return View(component);

            await _componentService.CreateComponentAsync(component);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var component =
                await _componentService.GetComponentByIdAsync(id);

            if (component == null)
                return NotFound();

            return View(component);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            [FromForm] ComponentDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var component = ComponentDTO.ConvertTo(dto);

            await _componentService.SaveComponentAsync(component);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var component =
                await _componentService.GetComponentByIdAsync(id);

            if (component == null)
                return NotFound();

            return PartialView("_DetailsPartial", component);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _componentService.DeleteComponentAsync(id);

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
                RequestId = Activity.Current?.Id ??
                            HttpContext.TraceIdentifier
            });
        }
    }
}