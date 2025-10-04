using MediatR;
using Attend.Application.Repositories;
using Attend.Domain.Entities;
using FluentValidation;

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

        // Email uniqueness validation
        var emailExists = await repository.ExistsByEmailAsync(
            request.Request.Email, 
            request.Request.Id.Value, 
            cancellationToken);
        if (emailExists)
            throw new ValidationException("Email already in use.");

        User.Update(user,
            name: request.Request.Name,
            email: request.Request.Email,
            phone: request.Request.Phone);

        await repository.UpdateAsync(user, cancellationToken);
        return true;
    }
}

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Request.Id)
            .NotEmpty()
            .WithMessage("User ID cannot be empty.");

        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty.");
        
        RuleFor(x => x.Request.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty.")
            .EmailAddress()
            .WithMessage("Invalid email format.");
            
        RuleFor(x => x.Request.Phone)
            .NotEmpty()
            .WithMessage("Phone cannot be empty.");
    }
}
