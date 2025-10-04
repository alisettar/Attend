using Attend.Application.Data;
using Attend.Domain.Entities;

namespace Attend.Application.Repositories;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Event>> GetAllAsync(CancellationToken cancellationToken);
    Task<(List<Event> items, long totalCount)> GetPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<List<Event>> GetUpcomingEventsAsync(CancellationToken cancellationToken);
    Task AddAsync(Event @event, CancellationToken cancellationToken);
    Task UpdateAsync(Event @event, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
