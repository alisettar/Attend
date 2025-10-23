using Attend.Domain.Entities;

namespace Attend.Application.Data.Attendances.Queries;

public record AttendanceResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid EventId,
    string EventTitle,
    bool CheckedIn,
    DateTime? CheckedInAt,
    string Status)
{
    public static AttendanceResponse FromDomain(Attendance attendance)
    {
        return new AttendanceResponse(
            attendance.Id,
            attendance.UserId,
            attendance.User?.Name ?? string.Empty,
            attendance.EventId,
            attendance.Event?.Title ?? string.Empty,
            attendance.CheckedIn,
            attendance.CheckedInAt,
            attendance.Status.ToString());
    }

    public static List<AttendanceResponse> FromDomainList(List<Attendance> attendances)
    {
        return attendances.Select(FromDomain).ToList();
    }
}
