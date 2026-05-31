using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Abstractions.Interfaces.Events;
using TPI_2026.Domain.Events;
using TPI_2026.Domain.Entities;
using TPI_2026.Infrastructure.Persistance;
using TPI_2026.Infrastructure.Persistance.Repositories;
using TPI_2026.Infrastructure.Persistance.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace TPI_2026.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<DbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddTransient<IEmailService, EmailService>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}