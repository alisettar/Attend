using Attend.Api.Middleware;
using Attend.Application;
using Attend.Domain.Configuration;
using Attend.Infrastructure;
using Attend.Infrastructure.Persistence;
using Carter;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<TenantsConfiguration>(
    builder.Configuration.GetSection("TenantsConfiguration"));

builder.Services.AddDbContext<AttendDbContext>((serviceProvider, options) =>
{
    var tenantService = serviceProvider.GetRequiredService<Attend.Application.Interfaces.ITenantService>();
    var connectionString = tenantService.GetConnectionString();

    if (string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("Tenant connection string not found!");

    options.UseSqlite(connectionString);
}, ServiceLifetime.Scoped);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddCarter();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins("https://gencligianlamasanati.azurewebsites.net/")
                  .AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        }
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>().AddProblemDetails();

var app = builder.Build();

// Initialize databases
using (var scope = app.Services.CreateScope())
{
    var tenantsConfig = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<TenantsConfiguration>>();

    foreach (var (tenantId, tenantConfig) in tenantsConfig.Value.Tenants)
    {
        Console.WriteLine($"🔄 {tenantConfig.Name} ({tenantId})");

        try
        {
            using var tenantScope = app.Services.CreateScope();
            var tenantService = tenantScope.ServiceProvider.GetRequiredService<Attend.Application.Interfaces.ITenantService>();
            tenantService.SetTenantId(tenantId);

            var context = tenantScope.ServiceProvider.GetRequiredService<AttendDbContext>();

            // Ensure database exists
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine($"  ✅ Database ready");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ {ex.Message}");
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<Attend.Infrastructure.Middleware.TenantMiddleware>();
app.UseCors();
app.UseExceptionHandler();
app.MapCarter();

app.Run();
