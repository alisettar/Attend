namespace Attend.Application.Data.Events.Commands;

public record EventRequest(
    Guid? Id,
    string Title,
    string Description,
    DateTime Date);
