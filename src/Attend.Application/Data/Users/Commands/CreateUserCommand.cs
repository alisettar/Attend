using Attend.Application.Repositories;
using Attend.Application.Services;
using Attend.Domain.Entities;
using FluentValidation;
using MediatR;
using System.Text.RegularExpressions;

namespace Attend.Application.Data.Users.Commands;

public sealed record CreateUserCommand(UserRequest Request) : IRequest<Guid>;

public sealed class CreateUserCommandHandler(IUserRepository repository, IQRCodeService qrCodeService)
    : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Normalize phone (remove spaces, dashes)
        var normalizedPhone = NormalizePhone(request.Request.Phone);

        // Email uniqueness validation (only if email is provided)
        if (!string.IsNullOrWhiteSpace(request.Request.Email))
        {
            var emailExists = await repository.ExistsByEmailAsync(request.Request.Email, null, cancellationToken);
            if (emailExists)
                throw new ValidationException("This email address is already in use.");
        }

        // Phone uniqueness validation
        if (!string.IsNullOrWhiteSpace(normalizedPhone))
        {
            var phoneExists = await repository.ExistsByPhoneAsync(normalizedPhone, null, cancellationToken);
            if (phoneExists)
                throw new ValidationException("This phone number is already in use.");
        }

        var user = User.Create(
            name: request.Request.Name,
            phone: normalizedPhone!,
            email: request.Request.Email);

        user.QRCodeImage = qrCodeService.GenerateQRCodeImage(user.QRCode);

        await repository.AddAsync(user, cancellationToken);
        return user.Id;
    }

    private static string? NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return phone;

        // Remove spaces, dashes, parentheses
        return Regex.Replace(phone, @"[\s\-\(\)]", "");
    }
}

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private static readonly Regex TurkishPhoneRegex = new(
        @"^(\+90|0)?5\d{9}$",
        RegexOptions.Compiled);

    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Request.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Must(phone =>
            {
                if (string.IsNullOrWhiteSpace(phone))
                    return false;

                // Normalize and validate
                var normalized = Regex.Replace(phone, @"[\s\-\(\)]", "");
                return TurkishPhoneRegex.IsMatch(normalized);
            })
            .WithMessage("Please enter a valid Turkish phone number (e.g., 05XX XXX XX XX)");

        RuleFor(x => x.Request.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Email))
            .WithMessage("Please enter a valid email address.");
    }
}
