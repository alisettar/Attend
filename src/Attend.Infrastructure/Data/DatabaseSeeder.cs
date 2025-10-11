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

        // Determine tenant from connection string
        var connectionString = context.Database.GetConnectionString();
        var fileName = connectionString?.Contains("Erkekler") == true 
            ? "participants_men.json" 
            : "participants_women.json";
        
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"❌ Seed file not found: {jsonPath}");
            return;
        }
        
        Console.WriteLine($"📂 Loading seed data from: {fileName}");

        var json = await File.ReadAllTextAsync(jsonPath);
        Console.WriteLine($"📄 JSON file read, length: {json.Length}");
        
        var data = JsonSerializer.Deserialize<ParticipantsData>(json);
        Console.WriteLine($"📊 Deserialized data - TotalCount: {data?.TotalCount}, Names count: {data?.Names?.Count}");

        if (data?.Names == null || data.Names.Count == 0)
        {
            Console.WriteLine($"⚠️ No names found in JSON data!");
            return;
        }

        Console.WriteLine($"👥 Creating {data.Names.Count} users...");
        var users = data.Names.Select(name => User.Create(name)).ToList();

        Console.WriteLine($"💾 Adding users to context...");
        await context.Users.AddRangeAsync(users);
        
        Console.WriteLine($"💾 Saving changes...");
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
        public List<string> Names { get; set; } = [];
    }
}
