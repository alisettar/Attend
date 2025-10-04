using Attend.Application.Data;
using Attend.Application.Repositories;
using Attend.Domain.Entities;
using Attend.Domain.Enums;
using Attend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Attend.Infrastructure.Repositories;

public class AttendanceRepository(AttendDbContext context) : IAttendanceRepository
{
    public async Task<Attendance?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Attendances
            .Include(a => a.User)
            .Include(a => a.Event)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<Attendance?> GetByUserAndEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken)
        => await context.Attendances
            .FirstOrDefaultAsync(a => a.UserId == userId && a.EventId == eventId, cancellationToken);

    public async Task<Attendance?> GetActiveAttendanceByUserAsync(Guid userId, CancellationToken cancellationToken)
        => await context.Attendances
            .Include(a => a.Event)
            .Where(a => a.UserId == userId && a.Status == AttendanceStatus.Registered)
            .OrderByDescending(a => a.Event.Date)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<List<Attendance>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken)
        => await context.Attendances
            .Include(a => a.User)
            .Where(a => a.EventId == eventId)
            .OrderBy(a => a.User.Name)
            .ToListAsync(cancellationToken);

    public async Task<List<Attendance>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => await context.Attendances
            .Include(a => a.Event)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Event.Date)
            .ToListAsync(cancellationToken);

    public async Task<(List<Attendance> items, long totalCount)> GetPaginatedByEventAsync(
        Guid eventId, 
        PaginationRequest request, 
        CancellationToken cancellationToken = default)
    {
        var query = context.Attendances
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Event)
            .Where(a => a.EventId == eventId);

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            query = query.Where(a => 
                a.User.Name.Contains(request.SearchText) ||
                (a.User.Email != null && a.User.Email.Contains(request.SearchText)));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        query = request.OrderBy switch
        {
            "UserName" => request.OrderDescending ? query.OrderByDescending(a => a.User.Name) : query.OrderBy(a => a.User.Name),
            "CheckedIn" => request.OrderDescending ? query.OrderByDescending(a => a.CheckedIn) : query.OrderBy(a => a.CheckedIn),
            _ => query.OrderBy(a => a.User.Name)
        };

        var items = await query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(List<Attendance> items, long totalCount)> GetPaginatedByUserAsync(
        Guid userId, 
        PaginationRequest request, 
        CancellationToken cancellationToken = default)
    {
        var query = context.Attendances
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Event)
            .Where(a => a.UserId == userId);

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            query = query.Where(a => 
                a.Event.Title.Contains(request.SearchText) ||
                (a.Event.Description != null && a.Event.Description.Contains(request.SearchText)));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        query = request.OrderBy switch
        {
            "EventTitle" => request.OrderDescending ? query.OrderByDescending(a => a.Event.Title) : query.OrderBy(a => a.Event.Title),
            "EventDate" => request.OrderDescending ? query.OrderByDescending(a => a.Event.Date) : query.OrderBy(a => a.Event.Date),
            _ => query.OrderByDescending(a => a.Event.Date)
        };

        var items = await query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Attendance attendance, CancellationToken cancellationToken)
    {
        context.Attendances.Add(attendance);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Attendance attendance, CancellationToken cancellationToken)
    {
        context.Attendances.Update(attendance);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var attendance = await GetByIdAsync(id, cancellationToken);
        if (attendance != null)
        {
            context.Attendances.Remove(attendance);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
