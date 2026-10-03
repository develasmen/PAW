using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAW.Models;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(
            ISupplierService supplierService,
            ILogger<SupplierController> logger)
        {
            _supplierService = supplierService;
            _logger = logger;
        }

      
        public async Task<IActionResult> Index(int page = 1)
        {
            var suppliers = await _supplierService.GetSuppliersAsync();

            const int pageSize = 25;

            var totalItems = suppliers.Count();

            if (page < 1)
                page = 1;

            var items = suppliers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedResult<SupplierDTO>
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
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return View(supplier);

            await _supplierService.SaveSupplierAsync(supplier);

            return RedirectToAction(nameof(Index));
        }

      
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);

            if (supplier == null)
                return NotFound();

            return View(supplier);
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] SupplierDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var supplier = SupplierDTO.ConvertTo(dto);

            await _supplierService.SaveSupplierAsync(supplier);

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);

            if (supplier == null)
                return NotFound();

            return PartialView("_DetailsPartial", supplier);
        }

    
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _supplierService.DeleteSupplierAsync(id);

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
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}