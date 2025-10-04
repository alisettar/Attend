using MediatR;
using Attend.Application.Repositories;

namespace Attend.Application.Data.Events.Queries;

public sealed record GetEventByIdQuery(Guid Id) : IRequest<EventResponse?>;

public sealed class GetEventByIdQueryHandler(IEventRepository repository) 
    : IRequestHandler<GetEventByIdQuery, EventResponse?>
{
    public async Task<EventResponse?> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var @event = await repository.GetByIdAsync(request.Id, cancellationToken);
        return @event != null ? EventResponse.FromDomain(@event) : null;
    }
}
