using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Abstractions.Interfaces.Events;
using TPI_2026.Domain.Events;
using TPI_2026.Domain.Entities;
using TPI_2026.Infrastructure.Persistence;
using TPI_2026.Infrastructure.Persistence.Repositories;
using TPI_2026.Infrastructure.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;
using Polly;
using Polly.Extensions.Http;

namespace TPI_2026.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<DbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IMedicalHistoryRepository, MedicalHistoryRepository>();
        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddScoped<IPasswordHasher<Doctor>, BCryptPasswordHasher<Doctor>>();
        services.AddScoped<IPasswordHasher<Receptionist>, BCryptPasswordHasher<Receptionist>>();
        services.AddScoped<IPasswordHasher<Administrator>, BCryptPasswordHasher<Administrator>>();

        services.AddHttpClient("ResendClient", client =>
        {
            client.BaseAddress = new Uri("https://api.resend.com/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["EmailSettings:ApiKey"]);
        })
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

        services.AddTransient<IEmailService, EmailService>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}