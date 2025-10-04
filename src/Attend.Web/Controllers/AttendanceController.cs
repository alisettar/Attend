using Microsoft.AspNetCore.Mvc;
using Attend.Web.Services.Interfaces;

namespace Attend.Web.Controllers;

public class AttendanceController(IAttendanceService attendanceService, IEventService eventService) : Controller
{
    public IActionResult Scanner()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> CheckIn(Guid eventId, Guid userId)
    {
        try
        {
            var result = await attendanceService.CheckInAsync(eventId, userId);
            
            if (result)
            {
                var eventDetails = await eventService.GetEventByIdAsync(eventId);
                TempData["Success"] = $"Successfully checked in to {eventDetails?.Title}!";
                return Json(new { success = true, message = "Check-in successful!" });
            }
            
            TempData["Error"] = "Check-in failed. Please try again.";
            return Json(new { success = false, message = "Check-in failed" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ProcessQRCode([FromBody] QRCodeRequest request)
    {
        try
        {
            // QR format: "EVENT-{eventId}|USER-{userId}"
            var parts = request.QRCode.Split('|');
            if (parts.Length != 2)
                return Json(new { success = false, message = "Invalid QR code format" });

            var eventId = Guid.Parse(parts[0].Replace("EVENT-", ""));
            var userId = Guid.Parse(parts[1].Replace("USER-", ""));

            var result = await attendanceService.CheckInAsync(eventId, userId);
            
            if (result)
            {
                var eventDetails = await eventService.GetEventByIdAsync(eventId);
                return Json(new 
                { 
                    success = true, 
                    message = $"Successfully checked in!",
                    eventTitle = eventDetails?.Title,
                    eventDate = eventDetails?.Date
                });
            }
            
            return Json(new { success = false, message = "Check-in failed" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }
}

public record QRCodeRequest(string QRCode);
