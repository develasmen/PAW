using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var users = await _userService.GetUsersAsync();
            const int pageSize = 25;
            page = Math.Max(page, 1);

            return View(new PagedResult<UserDTO>
            {
                Items = users.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = users.Count()
            });
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Email,RoleId,IsActive,ModifiedBy")] UserDTO dto)
        {
            ValidateUser(dto);
            if (!ModelState.IsValid)
                return View(dto);

            if (!await _userService.SaveUserAsync(dto))
            {
                ModelState.AddModelError("", "Could not save the user.");
                return View(dto);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound() : View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Email,RoleId,IsActive,ModifiedBy")] UserDTO dto)
        {
            if (id != dto.UserId)
                return BadRequest();
            ValidateUser(dto);
            if (!ModelState.IsValid)
                return View(dto);

            if (!await _userService.SaveUserAsync(dto))
            {
                ModelState.AddModelError("", "Could not save the user.");
                return View(dto);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound() : PartialView("_DetailsPartial", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private void ValidateUser(UserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                ModelState.AddModelError(nameof(dto.Username), "Username is required.");
            if (string.IsNullOrWhiteSpace(dto.Email))
                ModelState.AddModelError(nameof(dto.Email), "Email is required.");
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
