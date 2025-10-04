using MediatR;
using Attend.Application.Repositories;
using FluentValidation;

namespace Attend.Application.Data.Attendances.Commands;

public sealed record CheckInByQRCodeCommand(string QRCode) : IRequest<bool>;

public sealed class CheckInByQRCodeCommandHandler(
    IUserRepository userRepository,
    IAttendanceRepository attendanceRepository) 
    : IRequestHandler<CheckInByQRCodeCommand, bool>
{
    public async Task<bool> Handle(CheckInByQRCodeCommand request, CancellationToken cancellationToken)
    {
        // Get user by QR code
        var user = await userRepository.GetByQRCodeAsync(request.QRCode, cancellationToken);
        if (user == null)
            throw new ValidationException("Invalid QR code.");

        // Find active attendance for this user
        var attendance = await attendanceRepository.GetActiveAttendanceByUserAsync(user.Id, cancellationToken);
        if (attendance == null)
            throw new ValidationException("No active event registration found for this user.");

        attendance.CheckIn();
        await attendanceRepository.UpdateAsync(attendance, cancellationToken);
        return true;
    }
}

public sealed class CheckInByQRCodeCommandValidator : AbstractValidator<CheckInByQRCodeCommand>
{
    public CheckInByQRCodeCommandValidator()
    {
        RuleFor(x => x.QRCode)
            .NotEmpty()
            .WithMessage("QR code cannot be empty.");
    }
}
