using Microsoft.Extensions.DependencyInjection;

namespace TPI_2026.Presentation.Authorization;

public static class AuthorizationPolicies
{
    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdministratorOnly", policy => policy.RequireRole("Administrator"));
            options.AddPolicy("Staff", policy => policy.RequireRole("Receptionist", "Administrator"));
            options.AddPolicy("StaffAndDoctor", policy => policy.RequireRole("Receptionist", "Administrator", "Doctor"));

        });
    }

}