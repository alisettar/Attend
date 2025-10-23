using Attend.Application.Repositories;
using Attend.Domain.Entities;
using FluentValidation;
using MediatR;
using System.Text.RegularExpressions;

namespace Attend.Application.Data.Users.Commands;

public sealed record UpdateUserCommand(UserRequest Request) : IRequest<bool>;

public sealed class UpdateUserCommandHandler(IUserRepository repository)
    : IRequestHandler<UpdateUserCommand, bool>
{
    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (!request.Request.Id.HasValue)
            throw new ArgumentException("User ID is required");

        var user = await repository.GetByIdAsync(request.Request.Id.Value, cancellationToken);
        if (user == null)
            return false;

        // Normalize phone
        var normalizedPhone = NormalizePhone(request.Request.Phone);

        // Email uniqueness validation (only if email is provided)
        if (!string.IsNullOrWhiteSpace(request.Request.Email))
        {
            var emailExists = await repository.ExistsByEmailAsync(
                request.Request.Email,
                request.Request.Id.Value,
                cancellationToken);
            if (emailExists)
                throw new ValidationException("This email address is already in use.");
        }

        // Phone uniqueness validation
        if (!string.IsNullOrWhiteSpace(normalizedPhone))
        {
            var phoneExists = await repository.ExistsByPhoneAsync(
                normalizedPhone,
                request.Request.Id.Value,
                cancellationToken);
            if (phoneExists)
                throw new ValidationException("This phone number is already in use.");
        }

        User.Update(user,
            name: request.Request.Name,
            phone: normalizedPhone!,
            email: request.Request.Email);

        await repository.UpdateAsync(user, cancellationToken);
        return true;
    }

    private static string? NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return phone;

        // Remove spaces, dashes, parentheses
        return Regex.Replace(phone, @"[\s\-\(\)]", "");
    }
}

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private static readonly Regex TurkishPhoneRegex = new(
        @"^(\+90|0)?5\d{9}$",
        RegexOptions.Compiled);

    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Request.Id)
            .NotEmpty()
            .WithMessage("User ID is required.");

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
