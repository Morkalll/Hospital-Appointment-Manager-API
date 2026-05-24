using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TPI_2026.Application;
using TPI_2026.Application.Abstractions.Interfaces;
using TPI_2026.Infrastructure.Persistance;
using TPI_2026.Presentation.Authorization;
using TPI_2026.Domain.Entities;

namespace TPI_2026.Presentation.Infrastructure;

public static class ApplicationExtensions
{
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        builder.Services.AddRouting();
        builder.Services.AddEndpointsApiExplorer();
        return builder;
    }

    public static WebApplicationBuilder AddKeyVaultIfConfigured(this WebApplicationBuilder builder)
    {
        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationServices();
        return builder;
    }

    public static WebApplicationBuilder AddInfrastructureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(
                builder.Configuration.GetConnectionString("TPI_2026Db")
                ?? "Data Source=TPI_2026.db"));

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();
        builder.Services.AddScoped<ApplicationDbContext>();

        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return builder;
    }

    public static WebApplicationBuilder AddPresentationServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

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
        var endpointTypes = assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IEndpoint).IsAssignableFrom(t));

        foreach (var type in endpointTypes)
        {
            var endpoint = (IEndpoint)Activator.CreateInstance(type)!;
            endpoint.MapEndpoint(app);
        }

        return app;
    }

    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContextInitialiser>();
        await initializer.InitialiseAsync();
        await initializer.SeedAsync();
    }
}