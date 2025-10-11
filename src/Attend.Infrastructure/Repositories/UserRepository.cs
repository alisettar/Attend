using Microsoft.EntityFrameworkCore;
using Attend.Application.Data;
using Attend.Application.Repositories;
using Attend.Domain.Entities;
using Attend.Infrastructure.Persistence;

namespace Attend.Infrastructure.Repositories;

public class UserRepository(AttendDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Users
            .Include(u => u.Attendances)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => await context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<User?> GetByQRCodeAsync(string qrCode, CancellationToken cancellationToken)
        => await context.Users
            .FirstOrDefaultAsync(u => u.QRCode == qrCode, cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = context.Users.Where(u => u.Email == email);
        if (excludeUserId.HasValue)
            query = query.Where(u => u.Id != excludeUserId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
        => await context.Users
            .OrderBy(u => u.Name)
            .ToListAsync(cancellationToken);

    public async Task<(List<User> items, long totalCount)> GetPaginatedAsync(
        PaginationRequest request, 
        CancellationToken cancellationToken = default)
    {
        var query = context.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            var searchUpper = request.SearchText.ToUpperInvariant();
            query = query.Where(u => 
                u.Name.ToUpperInvariant().Contains(searchUpper) ||
                (u.Email != null && u.Email.ToUpperInvariant().Contains(searchUpper)) ||
                (u.Phone != null && u.Phone.ToUpperInvariant().Contains(searchUpper)));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        query = request.OrderBy switch
        {
            "Name" => request.OrderDescending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
            "Email" => request.OrderDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            _ => request.OrderDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt)
        };

        var items = await query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await GetByIdAsync(id, cancellationToken);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
