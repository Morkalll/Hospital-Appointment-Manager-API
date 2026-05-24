using Microsoft.Extensions.DependencyInjection;
using TPI_2026.Application.Abstractions.Interfaces;
using TPI_2026.Application.Services;

namespace TPI_2026.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
        return services;
    }
}