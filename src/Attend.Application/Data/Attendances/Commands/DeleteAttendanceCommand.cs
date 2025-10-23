using Attend.Application.Repositories;
using MediatR;

namespace Attend.Application.Data.Attendances.Commands;

public sealed record DeleteAttendanceCommand(Guid AttendanceId) : IRequest<bool>;

public sealed class DeleteAttendanceCommandHandler(IAttendanceRepository repository)
    : IRequestHandler<DeleteAttendanceCommand, bool>
{
    public async Task<bool> Handle(DeleteAttendanceCommand request, CancellationToken cancellationToken)
    {
        var attendance = await repository.GetByIdAsync(request.AttendanceId, cancellationToken);
        if (attendance == null)
            return false;

        await repository.DeleteAsync(attendance.Id, cancellationToken);
        return true;
    }
}
