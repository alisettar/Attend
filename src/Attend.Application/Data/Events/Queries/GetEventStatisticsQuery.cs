using Attend.Application.Repositories;
using MediatR;

namespace Attend.Application.Data.Events.Queries;

public sealed record GetEventStatisticsQuery(Guid EventId) : IRequest<EventStatisticsResponse>;

public sealed class GetEventStatisticsQueryHandler(IAttendanceRepository attendanceRepository)
    : IRequestHandler<GetEventStatisticsQuery, EventStatisticsResponse>
{
    public async Task<EventStatisticsResponse> Handle(
        GetEventStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var statistics = await attendanceRepository.GetEventStatisticsAsync(
            request.EventId,
            cancellationToken);

        return statistics;
    }
}

public record EventStatisticsResponse(
    int TotalRegistered,
    int TotalCheckedIn,
    int TotalCancelled
);
