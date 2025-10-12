using Microsoft.AspNetCore.Mvc;
using Attend.Web.Services.Interfaces;

namespace Attend.Web.Controllers;

public class ReportsController(IReportService reportService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var statistics = await reportService.GetDashboardStatisticsAsync();
        
        if (statistics == null)
        {
            ViewBag.Error = "Unable to load statistics.";
        }
        
        return View(statistics);
    }
}
