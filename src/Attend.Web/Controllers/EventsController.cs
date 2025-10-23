using Attend.Web.Exceptions;
using Attend.Web.Models;
using Attend.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Attend.Web.Controllers;

public class EventsController(
    IEventService eventService,
    IAttendanceService attendanceService,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string search = "")
    {
        try
        {
            ViewBag.SearchTerm = search;
            var request = new PaginationRequest(page - 1, 10, search);
            var events = await eventService.GetEventsAsync(request);
            return View(events);
        }
        catch (ApiException ex)
        {
            ViewBag.Error = ex.Message;
            return View(new PaginatedResponse<EventViewModel> { Items = [], TotalCount = 0 });
        }
    }

    public IActionResult Create()
    {
        return View(new EventCreateViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(EventCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await eventService.CreateEventAsync(model);
            TempData["Success"] = localizer["EventCreatedSuccessfully"].Value;
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
            var @event = await eventService.GetEventByIdAsync(id);
            if (@event == null)
                return NotFound();

            var model = new EventUpdateViewModel
            {
                Title = @event.Title,
                Description = @event.Description,
                Date = @event.Date
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
    public async Task<IActionResult> Edit(Guid id, EventUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var success = await eventService.UpdateEventAsync(id, model);
            if (success)
            {
                TempData["Success"] = localizer["EventUpdatedSuccessfully"].Value;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Error = localizer["EventNotFound"].Value;
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

    public async Task<IActionResult> Details(Guid id, int page = 1, string status = "")
    {
        try
        {
            var @event = await eventService.GetEventByIdAsync(id);
            if (@event == null)
                return NotFound();

            ViewBag.Event = @event;
            ViewBag.StatusFilter = status;

            var request = new PaginationRequest(page - 1, 10);
            var attendees = await attendanceService.GetEventAttendeesAsync(id, request);

            if (!string.IsNullOrEmpty(status))
            {
                var filteredItems = attendees.Items.Where(a => a.Status == status).ToList();
                attendees = new PaginatedResponse<AttendanceViewModel>
                {
                    Items = filteredItems,
                    TotalCount = filteredItems.Count,
                    Page = attendees.Page,
                    PageSize = attendees.PageSize
                };
            }

            return View(attendees);
        }
        catch (ApiException ex)
        {
            ViewBag.Error = ex.Message;
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await eventService.DeleteEventAsync(id);
            if (success)
                TempData["Success"] = localizer["EventDeletedSuccessfully"].Value;
            else
                TempData["Error"] = localizer["EventCouldNotBeDeleted"].Value;
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
