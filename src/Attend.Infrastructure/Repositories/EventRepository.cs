using Attend.Application.Data;
using Attend.Application.Repositories;
using Attend.Domain.Entities;
using Attend.Infrastructure.Extensions;
using Attend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Attend.Infrastructure.Repositories;

public class EventRepository(AttendDbContext context) : IEventRepository
{
    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Events
            .Include(e => e.Attendances)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<List<Event>> GetAllAsync(CancellationToken cancellationToken)
        => await context.Events
            .OrderByDescending(e => e.Date)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken)
        => await context.Events.CountAsync(cancellationToken);

    public async Task<List<Event>> GetUpcomingEventsAsync(CancellationToken cancellationToken)
        => await context.Events
            .Where(e => e.Date >= DateTime.UtcNow)
            .OrderBy(e => e.Date)
            .ToListAsync(cancellationToken);

    public async Task<(List<Event> items, long totalCount)> GetPaginatedAsync(
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = context.Events.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            // Client-side evaluation: Tüm kayıtları çek, sonra filtrele
            // Türkçe karakter ve case-insensitive arama için
            var allEvents = await query.ToListAsync(cancellationToken);
            var normalizedSearch = request.SearchText.NormalizeTurkish();
            
            var filteredEvents = allEvents.Where(e =>
                e.Title.NormalizeTurkish().Contains(normalizedSearch) ||
                (e.Description != null && e.Description.NormalizeTurkish().Contains(normalizedSearch))
            ).ToList();
            
            var totalCount = filteredEvents.Count;
            
            // Sıralama
            var sortedEvents = request.OrderBy switch
            {
                "Title" => request.OrderDescending 
                    ? filteredEvents.OrderByDescending(e => e.Title) 
                    : filteredEvents.OrderBy(e => e.Title),
                "Date" => request.OrderDescending 
                    ? filteredEvents.OrderByDescending(e => e.Date) 
                    : filteredEvents.OrderBy(e => e.Date),
                _ => request.OrderDescending 
                    ? filteredEvents.OrderByDescending(e => e.CreatedAt) 
                    : filteredEvents.OrderBy(e => e.CreatedAt)
            };
            
            // Pagination
            var paginatedItems = sortedEvents
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            return (paginatedItems, totalCount);
        }

        // Arama yapılmadıysa normal pagination
        var count = await query.LongCountAsync(cancellationToken);

        query = request.OrderBy switch
        {
            "Title" => request.OrderDescending ? query.OrderByDescending(e => e.Title) : query.OrderBy(e => e.Title),
            "Date" => request.OrderDescending ? query.OrderByDescending(e => e.Date) : query.OrderBy(e => e.Date),
            _ => request.OrderDescending ? query.OrderByDescending(e => e.CreatedAt) : query.OrderBy(e => e.CreatedAt)
        };

        var items = await query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, count);
    }

    public async Task AddAsync(Event @event, CancellationToken cancellationToken)
    {
        context.Events.Add(@event);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Event @event, CancellationToken cancellationToken)
    {
        context.Events.Update(@event);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var @event = await GetByIdAsync(id, cancellationToken);
        if (@event != null)
        {
            context.Events.Remove(@event);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
