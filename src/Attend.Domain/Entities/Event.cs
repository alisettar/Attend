using Attend.Domain.BaseClasses;

namespace Attend.Domain.Entities;

public class Event : Entity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<Attendance> Attendances { get; set; } = new();

    public static Event Create(
        string title,
        string description,
        DateTime date)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));

        return new Event
        {
            Title = title.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Date = date
        };
    }

    public static Event Update(
        Event currentEvent,
        string title,
        string description,
        DateTime date)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));

        currentEvent.Title = title.Trim();
        currentEvent.Description = description?.Trim() ?? string.Empty;
        currentEvent.Date = date;

        return currentEvent;
    }
}
