using Attend.Application.Repositories;
using Attend.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Attend.Application.Data.Attendances.Commands;

public sealed record RegisterAttendanceCommand(AttendanceRequest Request) : IRequest<Guid>;

public sealed class RegisterAttendanceCommandHandler(
    IAttendanceRepository attendanceRepository,
    IUserRepository userRepository,
    IEventRepository eventRepository)
    : IRequestHandler<RegisterAttendanceCommand, Guid>
{
    public async Task<Guid> Handle(RegisterAttendanceCommand request, CancellationToken cancellationToken)
    {
        // Check if user exists
        var user = await userRepository.GetByIdAsync(request.Request.UserId, cancellationToken);
        if (user == null)
            throw new ValidationException("Participant not found.");

        // Check if event exists
        var @event = await eventRepository.GetByIdAsync(request.Request.EventId, cancellationToken);
        if (@event == null)
            throw new ValidationException("Event not found.");

        // Check if already registered
        var existingAttendance = await attendanceRepository.GetByUserAndEventAsync(
            request.Request.UserId,
            request.Request.EventId,
            cancellationToken);
        if (existingAttendance != null)
            throw new ValidationException("Participant is already registered for this event.");

        var attendance = Attendance.Create(
            userId: request.Request.UserId,
            eventId: request.Request.EventId);

        await attendanceRepository.AddAsync(attendance, cancellationToken);
        return attendance.Id;
    }
}

public sealed class RegisterAttendanceCommandValidator : AbstractValidator<RegisterAttendanceCommand>
{
    public RegisterAttendanceCommandValidator()
    {
        RuleFor(x => x.Request.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Request.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
