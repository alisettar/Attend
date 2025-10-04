using Microsoft.AspNetCore.Mvc;
using Attend.Web.Services.Interfaces;

namespace Attend.Web.Controllers;

public class HomeController(IEventService eventService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var request = new Models.PaginationRequest(0, 10);
        var events = await eventService.GetEventsAsync(request);
        return View(events);
    }
}
