namespace Attend.Web.Models;

public record PaginationRequest(
    int Page = 0,
    int PageSize = 10,
    string SearchText = "",
    string OrderBy = "",
    bool OrderDescending = false);

public record PaginatedResponse<T>
{
    public List<T> Items { get; init; } = new();
    public long TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}

public record UserViewModel(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    string QRCode,
    string? QRCodeImage,
    DateTime CreatedAt,
    int AttendanceCount = 0);

public record UserCreateViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public record UserUpdateViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public record EventViewModel(
    Guid Id,
    string Title,
    string Description,
    DateTime Date,
    DateTime CreatedAt);

public record EventCreateViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now.AddDays(7);
}

public record EventUpdateViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public record AttendanceViewModel(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid EventId,
    string EventTitle,
    bool CheckedIn,
    DateTime? CheckedInAt,
    string Status);

public record CheckInResultViewModel(
    string UserName,
    bool IsNewCheckIn,
    string Status);
