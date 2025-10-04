namespace Attend.Application.Data.Attendances.Commands;

public record AttendanceRequest(
    Guid UserId,
    Guid EventId);
