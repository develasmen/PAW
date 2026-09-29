using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            var result = await _inventoryService.GetInventoriesAsync();

            return View(result);
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