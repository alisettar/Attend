using Microsoft.AspNetCore.Mvc;
using Attend.Web.Models;
using Attend.Web.Services.Interfaces;

namespace Attend.Web.Controllers;

public class UsersController(IUserService userService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string search = "")
    {
        ViewBag.SearchTerm = search;
        var request = new PaginationRequest(page - 1, 10, search);
        var users = await userService.GetUsersAsync(request);
        return View(users);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var user = await userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        return View(user);
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
            TempData["Success"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Error: {ex.Message}";
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
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
                TempData["Success"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Error = "User not found.";
            }
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Error: {ex.Message}";
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await userService.DeleteUserAsync(id);
            if (success)
                TempData["Success"] = "User deleted successfully.";
            else
                TempData["Error"] = "User could not be deleted.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
