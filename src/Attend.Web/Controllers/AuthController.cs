using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Attend.Web.Controllers;

public class AuthController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            ModelState.AddModelError("", "Kullanıcı adı gereklidir");
            return View();
        }

        var client = _httpClientFactory.CreateClient("AttendApi");
        var content = new StringContent(
            JsonSerializer.Serialize(new { username }),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("api/auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Kullanıcı bulunamadı");
            return View();
        }

        // Copy cookies from API response to current response
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var cookie in cookies)
            {
                Response.Headers.Append("Set-Cookie", cookie);
            }
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        var client = _httpClientFactory.CreateClient("AttendApi");
        await client.PostAsync("api/auth/logout", null);
        
        Response.Cookies.Delete("TenantId");
        Response.Cookies.Delete("Username");
        
        return RedirectToAction("Login");
    }
}
