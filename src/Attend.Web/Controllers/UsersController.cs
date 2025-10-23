using Attend.Web.Exceptions;
using Attend.Web.Models;
using Attend.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Attend.Web.Controllers;

public class UsersController(IUserService userService, IStringLocalizer<SharedResource> localizer) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string search = "")
    {
        try
        {
            ViewBag.SearchTerm = search;
            var request = new PaginationRequest(page - 1, 10, search);
            var users = await userService.GetUsersAsync(request);
            return View(users);
        }
        catch (ApiException ex)
        {
            ViewBag.Error = ex.Message;
            return View(new PaginatedResponse<UserViewModel> { Items = [], TotalCount = 0 });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersJson(int page = 1, int pageSize = 1000, string search = "")
    {
        try
        {
            var request = new PaginationRequest(page - 1, pageSize, search);
            var users = await userService.GetUsersAsync(request);
            return Json(users);
        }
        catch (ApiException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUserByIdJson(Guid id)
    {
        try
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Json(user);
        }
        catch (ApiException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    public async Task<IActionResult> Details(Guid id, int page = 1)
    {
        try
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var attendanceRequest = new PaginationRequest(page - 1, 10);
            var attendances = await userService.GetUserAttendancesAsync(id, attendanceRequest);

            ViewBag.Attendances = attendances;
            return View(user);
        }
        catch (ApiException ex)
        {
            ViewBag.Error = ex.Message;
            return NotFound();
        }
    }

    public IActionResult Create()
    {
        return View(new UserCreateViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await userService.CreateUserAsync(model);
            TempData["Success"] = localizer["UserCreatedSuccessfully"].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationApiException vex)
        {
            ModelState.AddModelError("", vex.Message);
            return View(model);
        }
        catch (ApiException ex)
        {
            ViewBag.Error = ex.Message;
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new UserUpdateViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone
            };

            return View(model);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UserUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var success = await userService.UpdateUserAsync(id, model);
            if (success)
            {
                TempData["Success"] = localizer["UserUpdatedSuccessfully"].Value;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Error = localizer["UserNotFound"].Value;
                return View(model);
            }
        }
        catch (ValidationApiException vex)
        {
            ModelState.AddModelError("", vex.Message);
            return View(model);
        }
        catch (ApiException ex)
        {
            ViewBag.Error = ex.Message;
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await userService.DeleteUserAsync(id);
            if (success)
                TempData["Success"] = localizer["UserDeletedSuccessfully"].Value;
            else
                TempData["Error"] = localizer["UserCouldNotBeDeleted"].Value;
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
