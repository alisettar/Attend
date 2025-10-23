using Microsoft.AspNetCore.Mvc;

namespace Attend.Web.Controllers;

public class LegalController : Controller
{
    [HttpGet("/privacy-policy")]
    public IActionResult PrivacyPolicy()
    {
        return View();
    }

    [HttpGet("/consent-text")]
    public IActionResult ConsentText()
    {
        return View();
    }
}
