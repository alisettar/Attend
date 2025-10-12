using MediatR;
using Attend.Application.Repositories;
using Attend.Application.Services;
using Attend.Domain.Entities;
using FluentValidation;

namespace Attend.Application.Data.Users.Commands;

public sealed record CreateUserCommand(UserRequest Request) : IRequest<Guid>;

public sealed class CreateUserCommandHandler(IUserRepository repository, IQRCodeService qrCodeService) 
    : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Email uniqueness validation (only if email is provided)
        if (!string.IsNullOrWhiteSpace(request.Request.Email))
        {
            var emailExists = await repository.ExistsByEmailAsync(request.Request.Email, null, cancellationToken);
            if (emailExists)
                throw new ValidationException("Email already in use.");
        }

        var user = User.Create(
            name: request.Request.Name,
            email: request.Request.Email,
            phone: request.Request.Phone);

        user.QRCodeImage = qrCodeService.GenerateQRCodeImage(user.QRCode);

        await repository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty.")
            .MaximumLength(200)
            .WithMessage("Name must not exceed 200 characters.");
        
        RuleFor(x => x.Request.Email)
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Email));
    }
}
