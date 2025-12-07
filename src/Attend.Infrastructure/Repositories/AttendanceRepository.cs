using Attend.Application.Data;
using Attend.Application.Repositories;
using Attend.Domain.Entities;
using Attend.Domain.Enums;
using Attend.Infrastructure.Extensions;
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

    public async Task<List<Attendance>> GetAllAsync(CancellationToken cancellationToken)
        => await context.Attendances
            .Include(a => a.User)
            .Include(a => a.Event)
            .ToListAsync(cancellationToken);

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
            // Client-side evaluation: Tüm kayıtları çek, sonra filtrele
            var allAttendances = await query.ToListAsync(cancellationToken);
            var normalizedSearch = request.SearchText.NormalizeTurkish();
            
            var filteredAttendances = allAttendances.Where(a =>
                a.User.Name.NormalizeTurkish().Contains(normalizedSearch) ||
                (a.User.Email != null && a.User.Email.NormalizeTurkish().Contains(normalizedSearch))
            ).ToList();
            
            var totalCount = filteredAttendances.Count;
            
            // Sıralama
            var sortedAttendances = request.OrderBy switch
            {
                "UserName" => request.OrderDescending 
                    ? filteredAttendances.OrderByDescending(a => a.User.Name) 
                    : filteredAttendances.OrderBy(a => a.User.Name),
                "CheckedIn" => request.OrderDescending 
                    ? filteredAttendances.OrderByDescending(a => a.CheckedIn) 
                    : filteredAttendances.OrderBy(a => a.CheckedIn),
                _ => filteredAttendances.OrderBy(a => a.User.Name)
            };
            
            // Pagination
            var paginatedItems = sortedAttendances
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            return (paginatedItems, totalCount);
        }

        // Arama yapılmadıysa normal pagination
        var count = await query.LongCountAsync(cancellationToken);

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

        return (items, count);
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
            // Client-side evaluation: Tüm kayıtları çek, sonra filtrele
            var allAttendances = await query.ToListAsync(cancellationToken);
            var normalizedSearch = request.SearchText.NormalizeTurkish();
            
            var filteredAttendances = allAttendances.Where(a =>
                a.Event.Title.NormalizeTurkish().Contains(normalizedSearch) ||
                (a.Event.Description != null && a.Event.Description.NormalizeTurkish().Contains(normalizedSearch))
            ).ToList();
            
            var totalCount = filteredAttendances.Count;
            
            // Sıralama
            var sortedAttendances = request.OrderBy switch
            {
                "EventTitle" => request.OrderDescending 
                    ? filteredAttendances.OrderByDescending(a => a.Event.Title) 
                    : filteredAttendances.OrderBy(a => a.Event.Title),
                "EventDate" => request.OrderDescending 
                    ? filteredAttendances.OrderByDescending(a => a.Event.Date) 
                    : filteredAttendances.OrderBy(a => a.Event.Date),
                _ => filteredAttendances.OrderByDescending(a => a.Event.Date)
            };
            
            // Pagination
            var paginatedItems = sortedAttendances
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            return (paginatedItems, totalCount);
        }

        // Arama yapılmadıysa normal pagination
        var count = await query.LongCountAsync(cancellationToken);

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

        return (items, count);
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

    public async Task<Application.Data.Events.Queries.EventStatisticsResponse> GetEventStatisticsAsync(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var attendances = await context.Attendances
            .Where(a => a.EventId == eventId)
            .ToListAsync(cancellationToken);

        var totalRegistered = attendances.Count;
        var totalCheckedIn = attendances.Count(a => a.Status == AttendanceStatus.CheckedIn);
        var totalCancelled = attendances.Count(a => a.Status == AttendanceStatus.Cancelled);

        return new Application.Data.Events.Queries.EventStatisticsResponse(
            totalRegistered,
            totalCheckedIn,
            totalCancelled);
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
