using Microsoft.Extensions.DependencyInjection;
using Attend.Application.Repositories;
using Attend.Infrastructure.Repositories;

namespace Attend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();

        return services;
    }
}
