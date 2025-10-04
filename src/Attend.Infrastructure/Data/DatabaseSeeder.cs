using Attend.Domain.Entities;
using Attend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Attend.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AttendDbContext context)
    {
        await SeedUsersAsync(context);
        await SeedEventsAsync(context);
    }

    private static async Task SeedUsersAsync(AttendDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "participants.json");
        
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"Seed file not found: {jsonPath}");
            return;
        }

        var json = await File.ReadAllTextAsync(jsonPath);
        var data = JsonSerializer.Deserialize<ParticipantsData>(json);

        if (data?.Names == null || !data.Names.Any())
            return;

        var users = data.Names.Select(name => User.Create(name)).ToList();

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ {users.Count} users seeded successfully.");
    }

    private static async Task SeedEventsAsync(AttendDbContext context)
    {
        if (await context.Events.AnyAsync())
            return;

        var events = new List<Event>
        {
            Event.Create("Ders 1", "2024-2025 Eğitim Dönemi Ders 1", new DateTime(2024, 10, 18, 19, 0, 0)),
            Event.Create("Ders 2", "2024-2025 Eğitim Dönemi Ders 2", new DateTime(2024, 11, 22, 19, 0, 0)),
            Event.Create("Ders 3", "2024-2025 Eğitim Dönemi Ders 3", new DateTime(2024, 12, 20, 19, 0, 0)),
            Event.Create("Ders 4", "2024-2025 Eğitim Dönemi Ders 4", new DateTime(2025, 1, 17, 19, 0, 0)),
            Event.Create("Ders 5", "2025-2026 Eğitim Dönemi Ders 5", new DateTime(2025, 3, 28, 19, 0, 0)),
            Event.Create("Ders 6", "2025-2026 Eğitim Dönemi Ders 6", new DateTime(2025, 5, 1, 19, 0, 0)),
            Event.Create("Ders 7", "2025-2026 Eğitim Dönemi Ders 7", new DateTime(2025, 6, 13, 19, 0, 0))
        };

        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ {events.Count} events seeded successfully.");
    }

    private class ParticipantsData
    {
        public int TotalCount { get; set; }
        public string? GeneratedAt { get; set; }
        public List<string> Names { get; set; } = new();
    }
}
