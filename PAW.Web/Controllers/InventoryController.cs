using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAW.Models;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(
            IInventoryService inventoryService,
            ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var inventories =
                await _inventoryService.GetInventoriesAsync();

            const int pageSize = 25;

            var totalItems = inventories.Count();

            if (page < 1)
                page = 1;

            var totalPages =
                (int)Math.Ceiling((double)totalItems / pageSize);

            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            var items = inventories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedResult<InventoryDTO>
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
        public async Task<IActionResult> Create(
            Inventory inventory)
        {
            if (!ModelState.IsValid)
                return View(inventory);

            await _inventoryService
                .CreateInventoryAsync(inventory);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var inventory =
                await _inventoryService.GetInventoryByIdAsync(id);

            if (inventory == null)
                return NotFound();

            return View(inventory);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            [FromForm] InventoryDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var inventory = InventoryDTO.ConvertTo(dto);

            await _inventoryService
                .SaveInventoryAsync(inventory);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var inventory =
                await _inventoryService.GetInventoryByIdAsync(id);

            if (inventory == null)
                return NotFound();

            return PartialView(
                "_DetailsPartial",
                inventory);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _inventoryService
                .DeleteInventoryAsync(id);

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