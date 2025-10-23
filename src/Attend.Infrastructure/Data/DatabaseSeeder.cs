using Attend.Domain.Entities;
using Attend.Infrastructure.Persistence;
using Attend.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Attend.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AttendDbContext context)
    {
        // Ensure database and tables are created first
        await context.Database.EnsureCreatedAsync();

        await SeedUsersAsync(context);
        await SeedEventsAsync(context);
    }

    private static async Task SeedUsersAsync(AttendDbContext context)
    {
        // Check if table exists first
        try
        {
            if (await context.Users.AnyAsync())
                return;
        }
        catch
        {
            // Table doesn't exist yet, continue with seeding
        }

        var connectionString = context.Database.GetConnectionString();
        var fileName = connectionString?.Contains("Erkekler") == true
            ? @"Data/participants_men.json"
            : @"Data/participants_women.json";

        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

        if (!File.Exists(jsonPath))
            return;

        var json = await File.ReadAllTextAsync(jsonPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var data = JsonSerializer.Deserialize<ParticipantsData>(json, options);

        if (data?.Names == null || data.Names.Count == 0)
            return;

        var qrService = new QRCodeService();
        var users = data.Names.Select((name, index) =>
        {
            // Generate unique dummy phone for seeding: 05000000001, 05000000002, etc.
            var phone = $"05{(index + 1):D9}";
            var user = User.Create(name, phone);
            user.QRCodeImage = qrService.GenerateQRCodeImage(user.QRCode);
            return user;
        }).ToList();

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ {users.Count} users seeded for {(fileName.Contains("men") ? "Erkekler" : "Kadınlar")}");
    }

    private static async Task SeedEventsAsync(AttendDbContext context)
    {
        // Check if table exists first
        try
        {
            if (await context.Events.AnyAsync())
                return;
        }
        catch
        {
            // Table doesn't exist yet, continue with seeding
        }

        var connectionString = context.Database.GetConnectionString();
        var fileName = connectionString?.Contains("Erkekler") == true
            ? @"Data/events_men.json"
            : @"Data/events_women.json";

        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

        if (!File.Exists(jsonPath))
            return;

        var json = await File.ReadAllTextAsync(jsonPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var data = JsonSerializer.Deserialize<EventsData>(json, options);

        if (data?.Events == null || data.Events.Count == 0)
            return;

        var events = data.Events.Select(e => Event.Create(e.Title, e.Description, e.Date)).ToList();
        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ {events.Count} events seeded for {(fileName.Contains("men") ? "Erkekler" : "Kadınlar")}");
    }

    private class ParticipantsData
    {
        public int TotalCount { get; set; }
        public string? GeneratedAt { get; set; }
        public List<string> Names { get; set; } = [];
    }

    private class EventsData
    {
        public int TotalCount { get; set; }
        public List<EventDto> Events { get; set; } = [];
    }

    private class EventDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
    }
}
