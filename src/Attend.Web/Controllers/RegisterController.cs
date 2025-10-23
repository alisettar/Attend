using Microsoft.AspNetCore.Mvc;
using Attend.Web.Models;
using System.Text;
using System.Text.Json;

namespace Attend.Web.Controllers;

public class RegisterController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public RegisterController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("AttendApi");
        _configuration = configuration;
    }

    [HttpGet("/register/{tenantHash}")]
    public IActionResult Index(string tenantHash)
    {
        ViewBag.TenantHash = tenantHash;
        ViewBag.RecaptchaSiteKey = _configuration["GoogleReCaptcha:SiteKey"];
        return View();
    }

    [HttpPost("/register/{tenantHash}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string tenantHash, RegisterFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.TenantHash = tenantHash;
            return View("Index", model);
        }

        try
        {
            var request = new { name = model.Name, phone = model.Phone };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"api/public/register/{tenantHash}", 
                content);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PublicRegisterResultDto>(resultJson, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return RedirectToAction("Success", new { 
                    userId = result?.UserId,
                    userName = result?.UserName
                });
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", errorMessage);
                ViewBag.TenantHash = tenantHash;
                return View("Index", model);
            }
        }
        catch (Exception ex)
        {
            // Log the actual error for debugging
            Console.WriteLine($"Registration error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            ModelState.AddModelError("", $"Kayıt işlemi başarısız: {ex.Message}");
            ViewBag.TenantHash = tenantHash;
            return View("Index", model);
        }
    }

    [HttpGet("/register/success")]
    public async Task<IActionResult> Success(Guid userId, string userName)
    {
        try
        {
            // Fetch QR code from API
            var response = await _httpClient.GetAsync($"users/{userId}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserDto>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                ViewBag.QRCodeImage = user?.QRCodeImage;
                ViewBag.UserName = userName;
                ViewBag.UserId = userId;
                
                return View();
            }
        }
        catch { }

        return RedirectToAction("Index", "Home");
    }
}
