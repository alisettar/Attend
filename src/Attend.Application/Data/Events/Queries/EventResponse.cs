using Attend.Domain.Entities;

namespace Attend.Application.Data.Events.Queries;

public record EventResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime Date,
    DateTime CreatedAt,
    int AttendeeCount)
{
    public static EventResponse FromDomain(Event @event)
    {
        return new EventResponse(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.Date,
            @event.CreatedAt,
            @event.Attendances.Count);
    }

    public static List<EventResponse> FromDomainList(List<Event> events)
    {
        return events.Select(FromDomain).ToList();
    }
}
