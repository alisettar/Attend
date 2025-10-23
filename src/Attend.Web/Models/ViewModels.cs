using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [RegularExpression(@"^0?5[0-9]{2}\s?[0-9]{3}\s?[0-9]{2}\s?[0-9]{2}$", ErrorMessage = "Invalid phone format. Use: 05XX XXX XX XX")]
    public string Phone { get; set; } = string.Empty;
}

public record UserUpdateViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [RegularExpression(@"^0?5[0-9]{2}\s?[0-9]{3}\s?[0-9]{2}\s?[0-9]{2}$", ErrorMessage = "Invalid phone format. Use: 05XX XXX XX XX")]
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

public record DashboardStatisticsViewModel(
    int TotalEvents,
    int TotalUsers,
    int TotalAttendances,
    int TotalCheckedIn,
    double CheckInRate,
    List<TopEventViewModel> TopEvents,
    List<TopUserViewModel> TopUsers);

public record TopEventViewModel(
    Guid Id,
    string Title,
    DateTime Date,
    int AttendanceCount,
    int CheckedInCount);

public record TopUserViewModel(
    Guid Id,
    string Name,
    int AttendanceCount,
    int CheckedInCount);
