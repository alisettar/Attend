using Attend.Application.Data.Public;
using Attend.Application.Data.Users.Queries;
using Attend.Application.Interfaces;
using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Attend.Api.Modules;

public class PublicModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Public endpoints - no authentication required
        app.MapPost("/api/public/register/{tenantHash}", RegisterPublic)
            .AllowAnonymous();
        
        app.MapGet("/api/public/user/by-phone/{tenantHash}", GetUserByPhone)
            .AllowAnonymous();
    }

    private static async Task<Results<Ok<PublicRegisterResult>, BadRequest<string>>> RegisterPublic(
        string tenantHash,
        [FromBody] PublicRegisterRequest request,
        [FromServices] ISender sender,
        HttpContext context)
    {
        try
        {
            Console.WriteLine($"[PublicRegister] TenantHash: {tenantHash}, Name: {request.Name}, Phone: {request.Phone}");
            var command = new PublicRegisterCommand(tenantHash, request);
            var result = await sender.Send(command, context.RequestAborted);
            Console.WriteLine($"[PublicRegister] Success: {result.UserId}");
            return TypedResults.Ok(result);
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"[PublicRegister] ValidationException: {ex.Message}");
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PublicRegister] Exception: {ex.Message}");
            Console.WriteLine($"[PublicRegister] StackTrace: {ex.StackTrace}");
            return TypedResults.BadRequest("Kayıt işlemi başarısız oldu. Lütfen tekrar deneyin.");
        }
    }

    private static async Task<Results<Ok<UserResponse>, NotFound>> GetUserByPhone(
        string tenantHash,
        [FromQuery] string phone,
        [FromServices] ISender sender,
        [FromServices] ITenantService tenantService,
        HttpContext context)
    {
        try
        {
            // Resolve tenant from hash
            var tenantId = tenantService.ResolveTenantByHash(tenantHash);
            if (tenantId == null)
            {
                return TypedResults.NotFound();
            }

            // Set tenant context for this request
            tenantService.SetTenantId(tenantId);

            var user = await sender.Send(new GetUserByPhoneQuery(phone), context.RequestAborted);
            return user is not null ? TypedResults.Ok(user) : TypedResults.NotFound();
        }
        catch
        {
            return TypedResults.NotFound();
        }
    }
}
