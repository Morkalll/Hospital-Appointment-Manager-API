using Microsoft.Extensions.DependencyInjection;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Abstractions.Interfaces.Events;
using TPI_2026.Application.Events;
using TPI_2026.Application.Services;
using TPI_2026.Domain.Events;

namespace TPI_2026.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddTransient<IEventHandler<AppointmentCreatedEvent>, SendEmailOnAppointmentCreatedHandler>();
        services.AddTransient<IEventHandler<AppointmentCanceledEvent>, SendEmailOnAppointmentCanceledHandler>();
        return services;
    }
}