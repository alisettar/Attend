using System.Collections.Concurrent;

namespace Attend.Web.Middleware;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, RateLimitInfo> _ipRequests = new();
    private const int MaxRequests = 5;
    private static readonly TimeSpan TimeWindow = TimeSpan.FromMinutes(1);

    public RateLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        var isRegisterPath = path?.StartsWith("/register") ?? false;

        if (isRegisterPath && context.Request.Method == "POST")
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var now = DateTime.UtcNow;
            var info = _ipRequests.GetOrAdd(ipAddress, _ => new RateLimitInfo());

            await info.Semaphore.WaitAsync();
            try
            {
                // Clean old requests
                info.Requests.RemoveAll(r => now - r > TimeWindow);

                if (info.Requests.Count >= MaxRequests)
                {
                    context.Response.StatusCode = 429;
                    await context.Response.WriteAsync("Çok fazla istek. Lütfen bir dakika sonra tekrar deneyin.");
                    return;
                }

                info.Requests.Add(now);
            }
            finally
            {
                info.Semaphore.Release();
            }
        }

        await _next(context);
    }

    private class RateLimitInfo
    {
        public List<DateTime> Requests { get; } = new();
        public SemaphoreSlim Semaphore { get; } = new(1, 1);
    }
}
