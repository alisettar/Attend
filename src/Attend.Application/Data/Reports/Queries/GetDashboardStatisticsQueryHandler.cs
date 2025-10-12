using MediatR;
using Attend.Application.Repositories;
using Attend.Domain.Enums;

namespace Attend.Application.Data.Reports.Queries;

public class GetDashboardStatisticsQueryHandler(
    IEventRepository eventRepository,
    IUserRepository userRepository,
    IAttendanceRepository attendanceRepository) 
    : IRequestHandler<GetDashboardStatisticsQuery, DashboardStatisticsResponse>
{
    public async Task<DashboardStatisticsResponse> Handle(
        GetDashboardStatisticsQuery request, 
        CancellationToken cancellationToken)
    {
        var totalEvents = await eventRepository.CountAsync(cancellationToken);
        var totalUsers = await userRepository.CountAsync(cancellationToken);
        
        var allAttendances = await attendanceRepository.GetAllAsync(cancellationToken);
        var totalAttendances = allAttendances.Count;
        var totalCheckedIn = allAttendances.Count(a => a.Status == AttendanceStatus.CheckedIn);
        
        var checkInRate = totalAttendances > 0 
            ? Math.Round((double)totalCheckedIn / totalAttendances * 100, 2) 
            : 0;

        // Top 5 Events
        var events = await eventRepository.GetAllAsync(cancellationToken);
        var topEvents = events
            .Select(e => new
            {
                Event = e,
                Attendances = allAttendances.Where(a => a.EventId == e.Id).ToList()
            })
            .Select(x => new TopEventResponse(
                x.Event.Id,
                x.Event.Title,
                x.Event.Date,
                x.Attendances.Count,
                x.Attendances.Count(a => a.Status == AttendanceStatus.CheckedIn)))
            .OrderByDescending(e => e.CheckedInCount)
            .Take(5)
            .ToList();

        // Top 5 Users
        var users = await userRepository.GetAllAsync(cancellationToken);
        var topUsers = users
            .Select(u => new
            {
                User = u,
                Attendances = allAttendances.Where(a => a.UserId == u.Id).ToList()
            })
            .Select(x => new TopUserResponse(
                x.User.Id,
                x.User.Name,
                x.Attendances.Count,
                x.Attendances.Count(a => a.Status == AttendanceStatus.CheckedIn)))
            .OrderByDescending(u => u.CheckedInCount)
            .Take(5)
            .ToList();

        return new DashboardStatisticsResponse(
            totalEvents,
            totalUsers,
            totalAttendances,
            totalCheckedIn,
            checkInRate,
            topEvents,
            topUsers);
    }
}
