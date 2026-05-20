using TPI_2026.Application.Abstractions.Interfaces;
using TPI_2026.Domain.Entities;
using TPI_2026.Infrastructure.Persistance;
using TPI_2026.Infrastructure.ExternalServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace TPI_2026.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=hospital.db";

        builder.Services.AddDbContext<ApplicationDbContext>
        (
            options => options.UseSqlite(connectionString)
        );

        builder.Services.AddScoped<IApplicationDbContext>
        (
            provider => provider.GetRequiredService<ApplicationDbContext>()
        );

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        builder.Services.AddTransient<IEmailSender<ApplicationUser>, NoOpEmailSender>();

        return builder;
    }
}

public class NoOpEmailSender : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        return Task.CompletedTask;
    }
}