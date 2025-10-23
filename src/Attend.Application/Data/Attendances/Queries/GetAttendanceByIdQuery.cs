using Attend.Application.Repositories;
using MediatR;

namespace Attend.Application.Data.Attendances.Queries;

public sealed record GetAttendanceByIdQuery(Guid AttendanceId)
    : IRequest<AttendanceResponse?>;

public sealed class GetAttendanceByIdQueryHandler(IAttendanceRepository repository)
    : IRequestHandler<GetAttendanceByIdQuery, AttendanceResponse?>
{
    public async Task<AttendanceResponse?> Handle(
        GetAttendanceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var attendance = await repository.GetByIdAsync(request.AttendanceId, cancellationToken);

        return attendance != null ? AttendanceResponse.FromDomain(attendance) : null;
    }
}
