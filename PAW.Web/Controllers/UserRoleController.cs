using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAW.Models;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    public class UserRoleController : Controller
    {
        private readonly IUserRoleService _userRoleService;
        private readonly ILogger<UserRoleController> _logger;

        public UserRoleController(IUserRoleService userRoleService, ILogger<UserRoleController> logger)
        {
            _userRoleService = userRoleService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var userRoles = await _userRoleService.GetUserRolesAsync();

            const int pageSize = 25;
            var totalItems = userRoles.Count();

            if (page < 1)
                page = 1;

            var items = userRoles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedResult<UserRoleDTO>
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
        public async Task<IActionResult> Create(UserRole userRole)
        {
            if (!ModelState.IsValid)
                return View(userRole);

            // UserRole tampoco tiene Id autoincremental, igual que UserAction.
            var existing = await _userRoleService.GetUserRolesAsync();
            userRole.Id = existing.Any() ? existing.Max(x => x.UserRoleId) + 1 : 1;

            await _userRoleService.CreateUserRoleAsync(userRole);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userRole = await _userRoleService.GetUserRoleByIdAsync(id);

            if (userRole == null)
                return NotFound();

            return View(userRole);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] UserRoleDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var userRole = UserRoleDTO.ConvertTo(dto);

            await _userRoleService.SaveUserRoleAsync(userRole);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userRole = await _userRoleService.GetUserRoleByIdAsync(id);

            if (userRole == null)
                return NotFound();

            return PartialView("_DetailsPartial", userRole);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _userRoleService.DeleteUserRoleAsync(id);

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}