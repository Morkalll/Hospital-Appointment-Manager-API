using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace TPI_2026.Presentation.Authorization;

public static class AuthorizationPolicies
{
    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Define authorization policies here.
            options.AddPolicy("DefaultPolicy", policy => policy.RequireAuthenticatedUser());
        });
    }
}
