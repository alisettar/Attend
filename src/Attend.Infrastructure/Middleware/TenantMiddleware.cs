using Attend.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Attend.Infrastructure.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        // Try header first, then cookie
        var tenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault()
                      ?? context.Request.Cookies["TenantId"];

        if (!string.IsNullOrEmpty(tenantId))
        {
            try
            {
                tenantService.SetTenantId(tenantId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid tenant: {TenantId}", tenantId);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync($"Invalid tenant: {tenantId}");
                return;
            }
        }

        await _next(context);
    }
}
