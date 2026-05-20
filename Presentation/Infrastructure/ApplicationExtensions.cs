using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TPI_2026.Application.Abstractions.Interfaces;
using TPI_2026.Infrastructure.Persistance;
using TPI_2026.Presentation.Authorization;

namespace TPI_2026.Presentation.Infrastructure;

public static class ApplicationExtensions
{
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        // Configure default services for the application.
        builder.Services.AddRouting();
        builder.Services.AddEndpointsApiExplorer();
        return builder;
    }

    public static WebApplicationBuilder AddKeyVaultIfConfigured(this WebApplicationBuilder builder)
    {
        // Optional Key Vault integration point.
        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        // Add application-layer services here.
        return builder;
    }

    public static WebApplicationBuilder AddInfrastructureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db"));

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();
        builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        return builder;
    }

    public static WebApplicationBuilder AddPresentationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationPolicies();
        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapGet("/health", () => Results.Ok("Healthy"));
        return app;
    }

    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        // Map additional endpoints from the supplied assembly if needed.
        return app;
    }

    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initializer.InitialiseAsync();
        await initializer.SeedAsync();
    }
}
