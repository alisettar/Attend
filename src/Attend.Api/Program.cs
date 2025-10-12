using Attend.Api.Middleware;
using Attend.Application;
using Attend.Domain.Configuration;
using Attend.Infrastructure;
using Attend.Infrastructure.Data;
using Attend.Infrastructure.Middleware;
using Attend.Infrastructure.Persistence;
using Carter;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//test
builder.Services.AddOpenApi();

// Configure Tenant Settings
builder.Services.Configure<TenantsConfiguration>(
    builder.Configuration.GetSection("TenantsConfiguration"));

// DbContext with dynamic connection string based on tenant
builder.Services.AddDbContext<AttendDbContext>((serviceProvider, options) =>
{
    var tenantService = serviceProvider.GetRequiredService<Attend.Application.Interfaces.ITenantService>();
    var connectionString = tenantService.GetConnectionString();
    
    if (string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("Tenant connection string not found! Ensure tenant is properly configured.");
    
    options.UseSqlite(connectionString);
}, ServiceLifetime.Scoped);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddCarter();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins("https://attend-web-ahmet.azurewebsites.net")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>().AddProblemDetails();

var app = builder.Build();

// Apply migrations and seed for ALL tenant databases
using (var scope = app.Services.CreateScope())
{
    var tenantsConfig = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<TenantsConfiguration>>();

    foreach (var (tenantId, tenantConfig) in tenantsConfig.Value.Tenants)
    {
        Console.WriteLine($"🔄 Processing tenant: {tenantConfig.Name} ({tenantId})");
        
        try
        {
            // Create a new scope for each tenant
            using var tenantScope = app.Services.CreateScope();
            var tenantService = tenantScope.ServiceProvider.GetRequiredService<Attend.Application.Interfaces.ITenantService>();
            tenantService.SetTenantId(tenantId);
            
            var context = tenantScope.ServiceProvider.GetRequiredService<AttendDbContext>();
            
            Console.WriteLine($"  🔄 Applying migrations...");
            await context.Database.MigrateAsync();
            Console.WriteLine($"  ✅ Migrations applied.");
            
            Console.WriteLine($"  🔄 Seeding database...");
            await DatabaseSeeder.SeedAsync(context);
            Console.WriteLine($"  ✅ Database seeded.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ Error: {ex.Message}");
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add tenant middleware BEFORE other middleware
app.UseMiddleware<TenantMiddleware>();

app.UseCors();
app.UseExceptionHandler();
app.MapCarter();

app.Run();
