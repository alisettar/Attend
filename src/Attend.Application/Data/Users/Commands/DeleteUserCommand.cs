using MediatR;
using Attend.Application.Repositories;

namespace Attend.Application.Data.Users.Commands;

public sealed record DeleteUserCommand(Guid Id) : IRequest<bool>;

public sealed class DeleteUserCommandHandler(IUserRepository repository) 
    : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
            return false;

        await repository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}
