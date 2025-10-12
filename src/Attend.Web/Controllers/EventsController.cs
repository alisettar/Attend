using Microsoft.AspNetCore.Mvc;
using Attend.Web.Models;
using Attend.Web.Services.Interfaces;

namespace Attend.Web.Controllers;

public class EventsController(IEventService eventService, IAttendanceService attendanceService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string search = "")
    {
        ViewBag.SearchTerm = search;
        var request = new PaginationRequest(page - 1, 10, search);
        var events = await eventService.GetEventsAsync(request);
        return View(events);
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
            TempData["Success"] = "Event created successfully.";
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
                TempData["Success"] = "Event updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Error = "Event not found.";
            }
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Error: {ex.Message}";
        }

        return View(model);
    }

    public async Task<IActionResult> Details(Guid id, int page = 1, string status = "")
    {
        var @event = await eventService.GetEventByIdAsync(id);
        if (@event == null)
            return NotFound();

        ViewBag.Event = @event;
        ViewBag.StatusFilter = status;
        
        var request = new PaginationRequest(page - 1, 10);
        var attendees = await attendanceService.GetEventAttendeesAsync(id, request);
        
        // Client-side filtering for now
        if (!string.IsNullOrEmpty(status))
        {
            attendees.Items = attendees.Items.Where(a => a.Status == status).ToList();
            attendees.TotalCount = attendees.Items.Count;
        }
        
        return View(attendees);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await eventService.DeleteEventAsync(id);
            if (success)
                TempData["Success"] = "Event deleted successfully.";
            else
                TempData["Error"] = "Event could not be deleted.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
