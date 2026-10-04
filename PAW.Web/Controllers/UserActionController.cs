using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAW.Models;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Services;

namespace PAW.Web.Controllers
{
    public class UserActionController : Controller
    {
        private readonly IUserActionService _userActionService;
        private readonly ILogger<UserActionController> _logger;

        public UserActionController(IUserActionService userActionService, ILogger<UserActionController> logger)
        {
            _userActionService = userActionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var userActions = await _userActionService.GetUserActionsAsync();

            const int pageSize = 25;
            var totalItems = userActions.Count();

            if (page < 1)
                page = 1;

            var items = userActions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedResult<UserActionDTO>
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
        public async Task<IActionResult> Create(UserAction userAction)
        {
            if (!ModelState.IsValid)
                return View(userAction);

            // UserAction no tiene Id autoincremental en la base, le sumamos 1 al id maximo 
            var existing = await _userActionService.GetUserActionsAsync();
            userAction.Id = existing.Any() ? existing.Max(x => x.UserActionId) + 1 : 1;

            await _userActionService.CreateUserActionAsync(userAction);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userAction = await _userActionService.GetUserActionByIdAsync(id);

            if (userAction == null)
                return NotFound();

            return View(userAction);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] UserActionDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var userAction = UserActionDTO.ConvertTo(dto);

            await _userActionService.SaveUserActionAsync(userAction);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userAction = await _userActionService.GetUserActionByIdAsync(id);

            if (userAction == null)
                return NotFound();

            return PartialView("_DetailsPartial", userAction);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _userActionService.DeleteUserActionAsync(id);

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}