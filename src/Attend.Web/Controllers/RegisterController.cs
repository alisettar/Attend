using Attend.Web.Exceptions;
using Attend.Web.Extensions;
using Attend.Web.Models;
using Microsoft.AspNetCore.Mvc;
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
        ViewBag.TenantName = GetTenantName(tenantHash);
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
            ViewBag.TenantName = GetTenantName(tenantHash);
            ViewBag.RecaptchaSiteKey = _configuration["GoogleReCaptcha:SiteKey"];
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
                qrCodeImage = result.QRCodeImage
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
        ViewBag.RecaptchaSiteKey = _configuration["GoogleReCaptcha:SiteKey"];
        return View("Index", model);
    }

    [HttpGet("/register/success")]
    public IActionResult Success(string tenantHash, Guid userId, string userName, string qrCodeImage)
    {
        ViewBag.TenantHash = tenantHash;
        ViewBag.TenantName = GetTenantName(tenantHash);
        ViewBag.QRCodeImage = qrCodeImage;
        ViewBag.UserName = userName;
        ViewBag.UserId = userId;

        return View();
    }

    private string GetTenantName(string tenantHash)
    {
        var tenants = _configuration.GetSection("TenantsConfiguration:Tenants").GetChildren();
        
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
}
