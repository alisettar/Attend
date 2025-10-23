using Attend.Application.Data;
using Attend.Application.Data.Events.Commands;
using Attend.Application.Data.Events.Queries;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Attend.Api.Modules;

public class EventsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/events/{id}", GetEventById);
        app.MapGet("/events", GetEvents);
        app.MapPost("/events", CreateEvent);
        app.MapPut("/events/{id}", UpdateEvent);
        app.MapDelete("/events/{id}", DeleteEvent);
    }

    private static async Task<Results<Ok<EventResponse>, NotFound>> GetEventById(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var @event = await sender.Send(new GetEventByIdQuery(id), context.RequestAborted);
        return @event is not null ? TypedResults.Ok(@event) : TypedResults.NotFound();
    }

    private static async Task<Ok<PaginationResponse<EventResponse>>> GetEvents(
        [FromQuery] PaginationRequest? paginationRequest,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(
            new GetEventsQuery(paginationRequest ?? new()),
            context.RequestAborted);
        return TypedResults.Ok(result);
    }

    private static async Task<Created> CreateEvent(
        EventRequest request,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var eventId = await sender.Send(new CreateEventCommand(request), context.RequestAborted);
        return TypedResults.Created($"/events/{eventId}");
    }

    private static async Task<Results<Ok, NotFound>> UpdateEvent(
        Guid id,
        EventRequest request,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var requestWithId = request with { Id = id };
        var result = await sender.Send(new UpdateEventCommand(requestWithId), context.RequestAborted);
        return result ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteEvent(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(new DeleteEventCommand(id), context.RequestAborted);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
