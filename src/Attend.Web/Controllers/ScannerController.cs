using Microsoft.AspNetCore.Mvc;
using Attend.Web.Models;
using Attend.Web.Services.Interfaces;

namespace Attend.Web.Controllers;

public class ScannerController(IEventService eventService, IAttendanceService attendanceService) : Controller
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
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public record CheckInRequest(string QRCode, Guid EventId);
