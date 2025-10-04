using Attend.Domain.BaseClasses;
using Attend.Domain.Enums;

namespace Attend.Domain.Entities;

public class Attendance : Entity
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public bool CheckedIn { get; private set; }
    public DateTime? CheckedInAt { get; private set; }
    public AttendanceStatus Status { get; private set; } = AttendanceStatus.Registered;

    // Navigation
    public User User { get; set; } = null!;
    public Event Event { get; set; } = null!;

    public static Attendance Create(Guid userId, Guid eventId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required", nameof(userId));
        if (eventId == Guid.Empty)
            throw new ArgumentException("Event ID is required", nameof(eventId));

        return new Attendance
        {
            UserId = userId,
            EventId = eventId,
            CheckedIn = false,
            Status = AttendanceStatus.Registered
        };
    }

    public void CheckIn()
    {
        if (CheckedIn)
            throw new InvalidOperationException("Already checked in");
        if (Status == AttendanceStatus.Cancelled)
            throw new InvalidOperationException("Cannot check in cancelled attendance");

        CheckedIn = true;
        CheckedInAt = DateTime.UtcNow;
        Status = AttendanceStatus.CheckedIn;
    }

    public void Cancel()
    {
        if (CheckedIn)
            throw new InvalidOperationException("Cannot cancel after check-in");

        Status = AttendanceStatus.Cancelled;
    }
}
