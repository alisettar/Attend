namespace Attend.Web.Middleware;

public class AuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        var isAuthPath = path?.StartsWith("/auth") ?? false;
        var isPublicPath = path?.StartsWith("/register") ?? false;
        var isLegalPath = (path?.StartsWith("/privacy-policy") ?? false) || (path?.StartsWith("/consent-text") ?? false);
        var isStaticFile = path?.Contains('.') ?? false;
        var hasCookie = context.Request.Cookies.ContainsKey("TenantId");

        if (!isAuthPath && !isPublicPath && !isLegalPath && !isStaticFile && !hasCookie)
        {
            context.Response.Redirect("/Auth/Login");
            return;
        }

        await next(context);
    }
}
