using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TPI_2026.Infrastructure.Persistance;

namespace TPI_2026.Presentation.Infrastructure;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        // Register default services for the application.
        return builder;
    }

    public static WebApplicationBuilder AddKeyVaultIfConfigured(this WebApplicationBuilder builder)
    {
        // Configure Azure Key Vault only if options are enabled in configuration.
        return builder;
    }

    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initializer.InitialiseAsync();
        await initializer.SeedAsync();
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Map any default endpoints if available.
        return app;
    }

    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        // Map endpoints from the provided assembly.
        return app;
    }
}
