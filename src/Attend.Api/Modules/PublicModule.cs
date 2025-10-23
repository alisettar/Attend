using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Attend.Application.Data.Public;
using FluentValidation;

namespace Attend.Api.Modules;

public class PublicModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Public endpoint - no authentication required
        app.MapPost("/api/public/register/{tenantHash}", RegisterPublic)
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
            var command = new PublicRegisterCommand(tenantHash, request);
            var result = await sender.Send(command, context.RequestAborted);
            return TypedResults.Ok(result);
        }
        catch (ValidationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest("Kayıt işlemi başarısız oldu. Lütfen tekrar deneyin.");
        }
    }
}
