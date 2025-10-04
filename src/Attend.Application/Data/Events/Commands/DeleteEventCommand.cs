using MediatR;
using Attend.Application.Repositories;

namespace Attend.Application.Data.Events.Commands;

public sealed record DeleteEventCommand(Guid Id) : IRequest<bool>;

public sealed class DeleteEventCommandHandler(IEventRepository repository) 
    : IRequestHandler<DeleteEventCommand, bool>
{
    public async Task<bool> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (@event == null)
            return false;

        await repository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}
