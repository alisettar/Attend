using MediatR;
using Attend.Application.Repositories;
using FluentValidation;
using Attend.Domain.Entities;

namespace Attend.Application.Data.Attendances.Commands;

public sealed record CheckInByQRCodeCommand(string QRCode, Guid EventId) : IRequest<CheckInResult>;

public sealed record CheckInResult(
    string UserName,
    bool IsNewCheckIn,
    string Status);

public sealed class CheckInByQRCodeCommandHandler(
    IUserRepository userRepository,
    IEventRepository eventRepository,
    IAttendanceRepository attendanceRepository) 
    : IRequestHandler<CheckInByQRCodeCommand, CheckInResult>
{
    public async Task<CheckInResult> Handle(CheckInByQRCodeCommand request, CancellationToken cancellationToken)
    {
        // Get user by QR code
        var user = await userRepository.GetByQRCodeAsync(request.QRCode, cancellationToken);
        if (user == null)
            throw new ValidationException("Invalid QR code.");

        // Verify event exists
        var evt = await eventRepository.GetByIdAsync(request.EventId, cancellationToken);
        if (evt == null)
            throw new ValidationException("Event not found.");

        // Check if attendance exists
        var attendance = await attendanceRepository.GetByUserAndEventAsync(user.Id, request.EventId, cancellationToken);

        if (attendance != null)
        {
            // Already registered
            if (attendance.CheckedIn)
            {
                return new CheckInResult(user.Name, false, "AlreadyCheckedIn");
            }
            else
            {
                // Update existing attendance
                attendance.CheckIn();
                await attendanceRepository.UpdateAsync(attendance, cancellationToken);
                return new CheckInResult(user.Name, false, "CheckedIn");
            }
        }
        else
        {
            // Create new attendance and check in
            var newAttendance = Attendance.Create(user.Id, request.EventId);
            newAttendance.CheckIn();
            await attendanceRepository.AddAsync(newAttendance, cancellationToken);
            return new CheckInResult(user.Name, true, "CheckedIn");
        }
    }
}

public sealed class CheckInByQRCodeCommandValidator : AbstractValidator<CheckInByQRCodeCommand>
{
    public CheckInByQRCodeCommandValidator()
    {
        RuleFor(x => x.QRCode)
            .NotEmpty()
            .WithMessage("QR code cannot be empty.");
        
        RuleFor(x => x.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");
    }
}
