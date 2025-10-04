using Carter;
using Microsoft.EntityFrameworkCore;
using Attend.Api.Middleware;
using Attend.Application;
using Attend.Infrastructure;
using Attend.Infrastructure.Persistence;
using Attend.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AttendDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddCarter();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>().AddProblemDetails();

var app = builder.Build();

// Auto-migrate and seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AttendDbContext>();
    try
    {
        Console.WriteLine("🔄 Applying migrations...");
        await context.Database.MigrateAsync();
        Console.WriteLine("✅ Migrations applied.");
        
        Console.WriteLine("🔄 Seeding database...");
        await DatabaseSeeder.SeedAsync(context);
        Console.WriteLine("✅ Database seeded.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Migration/Seed error: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.MapCarter();

app.Run();
