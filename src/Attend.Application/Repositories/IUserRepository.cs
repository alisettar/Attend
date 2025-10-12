using Attend.Application.Data;
using Attend.Domain.Entities;

namespace Attend.Application.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByQRCodeAsync(string qrCode, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<(List<User> items, long totalCount)> GetPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
