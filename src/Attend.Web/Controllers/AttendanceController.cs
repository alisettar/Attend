using Attend.Web.Exceptions;
using Attend.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Attend.Web.Controllers;

public class AttendanceController(
    IAttendanceService attendanceService,
    IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await attendanceService.DeleteAttendanceAsync(id);
            if (success)
                return Ok();
            else
                return NotFound(localizer["UserNotFound"].Value);
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

    [HttpPost]
    public async Task<IActionResult> Register(Guid eventId, [FromBody] Guid userId)
    {
        try
        {
            await attendanceService.RegisterAttendanceAsync(userId, eventId);
            return Ok(new { message = "Registration successful" });
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
        // Map API error messages to localization keys
        return apiError switch
        {
            "User is already registered for this event." => localizer["UserAlreadyRegisteredForEvent"].Value,
            "User not found." => localizer["UserNotFound"].Value,
            "Event not found." => localizer["EventNotFound"].Value,
            _ => apiError // Return original if no mapping found
        };
    }
}
