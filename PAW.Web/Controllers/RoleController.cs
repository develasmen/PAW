using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAW.Models;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var roles = await _roleService.GetRolesAsync();
            const int pageSize = 25;
            page = Math.Max(page, 1);

            return View(new PagedResult<RoleDTO>
            {
                Items = roles.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = roles.Count()
            });
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleName")] Role role)
        {
            if (string.IsNullOrWhiteSpace(role.RoleName))
                ModelState.AddModelError(nameof(role.RoleName), "Role name is required.");
            if (!ModelState.IsValid)
                return View(role);

            if (!await _roleService.SaveRoleAsync(role))
            {
                ModelState.AddModelError("", "Could not save the role.");
                return View(role);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            return role == null ? NotFound() : View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoleId,RoleName")] RoleDTO dto)
        {
            if (id != dto.RoleId)
                return BadRequest();
            if (string.IsNullOrWhiteSpace(dto.RoleName))
                ModelState.AddModelError(nameof(dto.RoleName), "Role name is required.");
            if (!ModelState.IsValid)
                return View(dto);

            if (!await _roleService.SaveRoleAsync(RoleDTO.ConvertTo(dto)))
            {
                ModelState.AddModelError("", "Could not save the role.");
                return View(dto);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            return role == null ? NotFound() : PartialView("_DetailsPartial", role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _roleService.DeleteRoleAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
