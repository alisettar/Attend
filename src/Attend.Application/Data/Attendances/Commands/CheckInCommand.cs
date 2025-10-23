using Attend.Application.Repositories;
using MediatR;

namespace Attend.Application.Data.Attendances.Commands;

public sealed record CheckInCommand(Guid AttendanceId) : IRequest<bool>;

public sealed class CheckInCommandHandler(IAttendanceRepository repository)
    : IRequestHandler<CheckInCommand, bool>
{
    public async Task<bool> Handle(CheckInCommand request, CancellationToken cancellationToken)
    {
        var attendance = await repository.GetByIdAsync(request.AttendanceId, cancellationToken);
        if (attendance == null)
            return false;

        attendance.CheckIn();
        await repository.UpdateAsync(attendance, cancellationToken);
        return true;
    }
}
