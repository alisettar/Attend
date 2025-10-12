using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Attend.Application.Data.Reports.Queries;

namespace Attend.Api.Modules;

public class ReportsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/reports").WithTags("Reports");
        group.MapGet("/statistics", GetDashboardStatistics);
    }

    private static async Task<Ok<DashboardStatisticsResponse>> GetDashboardStatistics(
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(new GetDashboardStatisticsQuery(), context.RequestAborted);
        return TypedResults.Ok(result);
    }
}
