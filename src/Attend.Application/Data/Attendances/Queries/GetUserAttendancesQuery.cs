using Attend.Application.Repositories;
using MediatR;

namespace Attend.Application.Data.Attendances.Queries;

public sealed record GetUserAttendancesQuery(Guid UserId, PaginationRequest? PaginationRequest = null)
    : IRequest<PaginationResponse<AttendanceResponse>>;

public sealed class GetUserAttendancesQueryHandler(IAttendanceRepository repository)
    : IRequestHandler<GetUserAttendancesQuery, PaginationResponse<AttendanceResponse>>
{
    public async Task<PaginationResponse<AttendanceResponse>> Handle(
        GetUserAttendancesQuery request,
        CancellationToken cancellationToken)
    {
        var paginationRequest = request.PaginationRequest ?? new PaginationRequest();
        var (attendances, totalCount) = await repository.GetPaginatedByUserAsync(
            request.UserId,
            paginationRequest,
            cancellationToken);

        var responseList = AttendanceResponse.FromDomainList(attendances);

        return new PaginationResponse<AttendanceResponse>(responseList, totalCount);
    }
}
