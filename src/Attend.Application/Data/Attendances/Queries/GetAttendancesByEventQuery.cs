using MediatR;
using Attend.Application.Repositories;

namespace Attend.Application.Data.Attendances.Queries;

public sealed record GetAttendancesByEventQuery(Guid EventId, PaginationRequest? PaginationRequest = null) 
    : IRequest<PaginationResponse<AttendanceResponse>>;

public sealed class GetAttendancesByEventQueryHandler(IAttendanceRepository repository) 
    : IRequestHandler<GetAttendancesByEventQuery, PaginationResponse<AttendanceResponse>>
{
    public async Task<PaginationResponse<AttendanceResponse>> Handle(
        GetAttendancesByEventQuery request, 
        CancellationToken cancellationToken)
    {
        var paginationRequest = request.PaginationRequest ?? new PaginationRequest();
        var (attendances, totalCount) = await repository.GetPaginatedByEventAsync(
            request.EventId, 
            paginationRequest, 
            cancellationToken);
        
        var responseList = AttendanceResponse.FromDomainList(attendances);

        return new PaginationResponse<AttendanceResponse>(responseList, totalCount);
    }
}
