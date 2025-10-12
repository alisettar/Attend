using MediatR;

namespace Attend.Application.Data.Reports.Queries;

public record GetDashboardStatisticsQuery : IRequest<DashboardStatisticsResponse>;

public record DashboardStatisticsResponse(
    int TotalEvents,
    int TotalUsers,
    int TotalAttendances,
    int TotalCheckedIn,
    double CheckInRate,
    List<TopEventResponse> TopEvents,
    List<TopUserResponse> TopUsers);

public record TopEventResponse(
    Guid Id,
    string Title,
    DateTime Date,
    int AttendanceCount,
    int CheckedInCount);

public record TopUserResponse(
    Guid Id,
    string Name,
    int AttendanceCount,
    int CheckedInCount);
