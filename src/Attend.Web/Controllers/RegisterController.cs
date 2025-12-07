using Attend.Web.Exceptions;
using Attend.Web.Extensions;
using Attend.Web.Models;
using Attend.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Attend.Web.Controllers;

public class RegisterController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    IPhoneCheckRateLimitService rateLimitService) : Controller
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AttendApi");

    [HttpGet("/register/{tenantHash}")]
    public IActionResult Index(string tenantHash)
    {
        ViewBag.TenantHash = tenantHash;
        ViewBag.TenantName = GetTenantName(tenantHash);
        ViewBag.RecaptchaSiteKey = configuration["GoogleReCaptcha:SiteKey"];
        return View();
    }

    [HttpPost("/register/{tenantHash}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string tenantHash, RegisterFormModel model, [FromForm] bool isExistingUser = false)
    {
        // Mevcut kullanıcı QR istiyor
        if (isExistingUser)
        {
            return await GetExistingUserQR(tenantHash, model.Phone);
        }

        // Yeni kayıt işlemi
        return await CreateNewUser(tenantHash, model);
    }

    private async Task<IActionResult> GetExistingUserQR(string tenantHash, string phone)
    {
        try
        {
            // Kullanıcıyı telefon ile getir
            var response = await _httpClient.GetAsync($"api/public/user/by-phone/{tenantHash}?phone={Uri.EscapeDataString(phone)}");
            
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Kaydınız bulunamadı. Lütfen bilgilerinizi kontrol edin.");
                ViewBag.TenantHash = tenantHash;
                ViewBag.TenantName = GetTenantName(tenantHash);
                ViewBag.RecaptchaSiteKey = configuration["GoogleReCaptcha:SiteKey"];
                return View("Index", new RegisterFormModel { Phone = phone });
            }

            var user = await response.ReadAsJsonOrThrowAsync<UserViewModel>();

            return RedirectToAction("Success", new
            {
                tenantHash = tenantHash,
                userId = user.Id,
                userName = user.Name,
                qrCodeImage = user.QRCodeImage,
                isExisting = true
            });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "QR kod getirme işlemi başarısız. Lütfen tekrar deneyin.");
            ViewBag.TenantHash = tenantHash;
            ViewBag.TenantName = GetTenantName(tenantHash);
            ViewBag.RecaptchaSiteKey = configuration["GoogleReCaptcha:SiteKey"];
            return View("Index", new RegisterFormModel { Phone = phone });
        }
    }

    private async Task<IActionResult> CreateNewUser(string tenantHash, RegisterFormModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.TenantHash = tenantHash;
            ViewBag.TenantName = GetTenantName(tenantHash);
            ViewBag.RecaptchaSiteKey = configuration["GoogleReCaptcha:SiteKey"];
            return View("Index", model);
        }

        try
        {
            var request = new
            {
                name = model.Name,
                phone = model.Phone,
                recaptchaToken = model.RecaptchaToken
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"api/public/register/{tenantHash}", content);
            var result = await response.ReadAsJsonOrThrowAsync<PublicRegisterResultDto>();

            return RedirectToAction("Success", new
            {
                tenantHash = tenantHash,
                userId = result.UserId,
                userName = result.UserName,
                qrCodeImage = result.QRCodeImage,
                isExisting = false
            });
        }
        catch (ValidationApiException vex)
        {
            ModelState.AddModelError("", vex.Message);
        }
        catch (ApiException aex)
        {
            ModelState.AddModelError("", aex.Message);
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Kayıt işlemi başarısız. Lütfen tekrar deneyin.");
        }

        ViewBag.TenantHash = tenantHash;
        ViewBag.TenantName = GetTenantName(tenantHash);
        ViewBag.RecaptchaSiteKey = configuration["GoogleReCaptcha:SiteKey"];
        return View("Index", model);
    }

    [HttpGet("/register/success")]
    public IActionResult Success(string tenantHash, Guid userId, string userName, string qrCodeImage, bool isExisting = false)
    {
        ViewBag.TenantHash = tenantHash;
        ViewBag.TenantName = GetTenantName(tenantHash);
        ViewBag.WhatsAppGroupUrl = GetWhatsAppGroupUrl(tenantHash);
        ViewBag.QRCodeImage = qrCodeImage;
        ViewBag.UserName = userName;
        ViewBag.UserId = userId;
        ViewBag.IsExisting = isExisting;

        return View();
    }

    [HttpGet("/register/{tenantHash}/check-phone")]
    public async Task<IActionResult> CheckPhone(string tenantHash, [FromQuery] string phone)
    {
        try
        {
            // Rate limit kontrolü
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var isAllowed = await rateLimitService.IsAllowedAsync(ipAddress, phone);
            
            if (!isAllowed)
            {
                return StatusCode(429, new { error = "Çok fazla istek gönderildi. Lütfen bir saat sonra tekrar deneyin." });
            }

            var response = await _httpClient.GetAsync($"api/public/user/by-phone/{tenantHash}?phone={Uri.EscapeDataString(phone)}");
            
            if (response.IsSuccessStatusCode)
            {
                var user = await response.ReadAsJsonOrThrowAsync<UserViewModel>();
                return Ok(user);
            }
            
            return NotFound();
        }
        catch
        {
            return NotFound();
        }
    }

    private string GetTenantName(string tenantHash)
    {
        var tenants = configuration.GetSection("TenantsConfiguration:Tenants").GetChildren();
        
        foreach (var tenant in tenants)
        {
            var hash = tenant["Hash"];
            if (hash == tenantHash)
            {
                return tenant["Name"] ?? "Bilinmeyen";
            }
        }

        return "Bilinmeyen";
    }

    private string? GetWhatsAppGroupUrl(string tenantHash)
    {
        var tenants = configuration.GetSection("TenantsConfiguration:Tenants").GetChildren();
        
        foreach (var tenant in tenants)
        {
            var hash = tenant["Hash"];
            if (hash == tenantHash)
            {
                return tenant["WhatsAppGroupUrl"];
            }
        }

        return null;
    }
}
