using Attend.Web.Models;

namespace Attend.Web.Services.Interfaces;

public interface IAttendanceService
{
    Task<AttendanceViewModel?> GetAttendanceByIdAsync(Guid id);
    Task<PaginatedResponse<AttendanceViewModel>> GetEventAttendeesAsync(Guid eventId, PaginationRequest request);
    Task<PaginatedResponse<AttendanceViewModel>> GetUserAttendancesAsync(Guid userId, PaginationRequest request);
    Task<Guid> RegisterAttendanceAsync(Guid userId, Guid eventId);
    Task<bool> CheckInAsync(Guid attendanceId);
    Task<bool> CheckInAsync(Guid eventId, Guid userId);
    Task<CheckInResultViewModel> CheckInByQRCodeAsync(string qrCode, Guid eventId);
    Task<bool> DeleteAttendanceAsync(Guid attendanceId);
}
