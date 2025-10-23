using Attend.Application.Interfaces;
using Attend.Application.Repositories;
using Attend.Application.Services;
using Attend.Infrastructure.Repositories;
using Attend.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Attend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddSingleton<IQRCodeService, QRCodeService>();

        // ReCaptcha service with HttpClient
        services.AddHttpClient<IReCaptchaService, ReCaptchaService>();

        return services;
    }
}
