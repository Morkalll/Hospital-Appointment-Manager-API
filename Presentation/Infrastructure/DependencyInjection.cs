using Microsoft.Extensions.DependencyInjection;
using TPI_2026.Presentation.Authorization;

namespace TPI_2026.Presentation.Infrastructure;


public static class DependencyInjection
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
    {
        // Register presentation-layer services here.
        services.AddEndpointsApiExplorer();
        services.AddAuthorizationPolicies();

        return services;
    }
}

