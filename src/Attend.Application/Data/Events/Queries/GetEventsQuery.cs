using Attend.Application.Repositories;
using MediatR;

namespace Attend.Application.Data.Events.Queries;

public sealed record GetEventsQuery(PaginationRequest? PaginationRequest = null) : IRequest<PaginationResponse<EventResponse>>;

public sealed class GetEventsQueryHandler(IEventRepository repository)
    : IRequestHandler<GetEventsQuery, PaginationResponse<EventResponse>>
{
    public async Task<PaginationResponse<EventResponse>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        var paginationRequest = request.PaginationRequest ?? new PaginationRequest();
        var (events, totalCount) = await repository.GetPaginatedAsync(paginationRequest, cancellationToken);

        var responseList = EventResponse.FromDomainList(events);

        return new PaginationResponse<EventResponse>(responseList, totalCount);
    }
}
