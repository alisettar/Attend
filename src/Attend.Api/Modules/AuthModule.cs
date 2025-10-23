using Attend.Application.Interfaces;
using Carter;

namespace Attend.Api.Modules;

public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth");

        group.MapPost("login", Login);
        group.MapPost("logout", Logout);
    }

    private async Task<IResult> Login(LoginRequest request, ITenantService tenantService, HttpContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return Results.BadRequest(new { error = "Username is required" });

        var tenantId = tenantService.ResolveTenantByUsername(request.Username);

        if (tenantId == null)
            return Results.NotFound(new { error = "User not found" });

        // Set cookies
        context.Response.Cookies.Append("TenantId", tenantId, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(8),
            Path = "/"
        });

        context.Response.Cookies.Append("Username", request.Username, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(8),
            Path = "/"
        });

        await Task.CompletedTask;
        return Results.Ok(new { username = request.Username, tenantId });
    }

    private IResult Logout(HttpContext context)
    {
        context.Response.Cookies.Delete("TenantId");
        context.Response.Cookies.Delete("Username");
        return Results.Ok();
    }
}

public record LoginRequest(string Username);
