using Attend.Application.Data;
using Attend.Domain.Entities;

namespace Attend.Application.Repositories;

public interface IAttendanceRepository
{
    Task<Attendance?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Attendance?> GetByUserAndEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken);
    Task<Attendance?> GetActiveAttendanceByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<Attendance>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
    Task<List<Attendance>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<(List<Attendance> items, long totalCount)> GetPaginatedByEventAsync(Guid eventId, PaginationRequest request, CancellationToken cancellationToken = default);
    Task<(List<Attendance> items, long totalCount)> GetPaginatedByUserAsync(Guid userId, PaginationRequest request, CancellationToken cancellationToken = default);
    Task AddAsync(Attendance attendance, CancellationToken cancellationToken);
    Task UpdateAsync(Attendance attendance, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
