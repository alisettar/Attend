using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Attend.Application.Data;
using Attend.Application.Data.Attendances.Commands;
using Attend.Application.Data.Attendances.Queries;

namespace Attend.Api.Modules;

public class AttendancesModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/attendances/{id}", GetAttendanceById);
        app.MapGet("/events/{eventId}/attendees", GetEventAttendees);
        app.MapGet("/users/{userId}/attendances", GetUserAttendances);
        app.MapPost("/events/{eventId}/register", RegisterAttendance);
        app.MapPost("/attendances/{id}/checkin", CheckIn);
        app.MapPost("/checkin/qrcode", CheckInByQRCode);
        app.MapDelete("/attendances/{id}", CancelAttendance);
    }

    private static async Task<Results<Ok<AttendanceResponse>, NotFound>> GetAttendanceById(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var attendance = await sender.Send(new GetAttendanceByIdQuery(id), context.RequestAborted);
        return attendance is not null ? TypedResults.Ok(attendance) : TypedResults.NotFound();
    }

    private static async Task<Ok<PaginationResponse<AttendanceResponse>>> GetEventAttendees(
        Guid eventId,
        [FromQuery] PaginationRequest? paginationRequest,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(
            new GetAttendancesByEventQuery(eventId, paginationRequest ?? new()), 
            context.RequestAborted);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<PaginationResponse<AttendanceResponse>>> GetUserAttendances(
        Guid userId,
        [FromQuery] PaginationRequest? paginationRequest,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(
            new GetUserAttendancesQuery(userId, paginationRequest ?? new()), 
            context.RequestAborted);
        return TypedResults.Ok(result);
    }

    private static async Task<Created> RegisterAttendance(
        Guid eventId,
        [FromBody] Guid userId,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var request = new AttendanceRequest(userId, eventId);
        var attendanceId = await sender.Send(new RegisterAttendanceCommand(request), context.RequestAborted);
        return TypedResults.Created($"/attendances/{attendanceId}");
    }

    private static async Task<Results<Ok, NotFound>> CheckIn(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(new CheckInCommand(id), context.RequestAborted);
        return result ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Ok> CheckInByQRCode(
        [FromBody] string qrCode,
        [FromServices] ISender sender,
        HttpContext context)
    {
        await sender.Send(new CheckInByQRCodeCommand(qrCode), context.RequestAborted);
        return TypedResults.Ok();
    }

    private static async Task<Results<NoContent, NotFound>> CancelAttendance(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(new CancelAttendanceCommand(id), context.RequestAborted);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
