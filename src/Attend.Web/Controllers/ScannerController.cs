using Attend.Web.Exceptions;
using Attend.Web.Models;
using Attend.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Attend.Web.Controllers;

public class ScannerController(
    IEventService eventService,
    IAttendanceService attendanceService,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    public async Task<IActionResult> Index(Guid? eventId = null)
    {
        // Get all events for dropdown
        var request = new PaginationRequest(0, 100); // Get first 100 events
        var events = await eventService.GetEventsAsync(request);

        ViewBag.Events = events.Items;
        ViewBag.SelectedEventId = eventId;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
    {
        try
        {
            var result = await attendanceService.CheckInByQRCodeAsync(request.QRCode, request.EventId);
            return Ok(result);
        }
        catch (ValidationApiException vex)
        {
            var localizedError = LocalizeApiError(vex.Message);
            return BadRequest(new { error = localizedError });
        }
        catch (ApiException aex)
        {
            var localizedError = LocalizeApiError(aex.Message);
            return StatusCode(aex.StatusCode, new { error = localizedError });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private string LocalizeApiError(string apiError)
    {
        return apiError switch
        {
            "User is already registered for this event." => localizer["UserAlreadyRegisteredForEvent"].Value,
            "User not found." => localizer["UserNotFound"].Value,
            "Event not found." => localizer["EventNotFound"].Value,
            _ => apiError
        };
    }
}

public record CheckInRequest(string QRCode, Guid EventId);
