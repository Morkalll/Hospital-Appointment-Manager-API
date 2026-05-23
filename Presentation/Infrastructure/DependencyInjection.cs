using Microsoft.Extensions.DependencyInjection;

namespace TPI_2026.Presentation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
    {
        // Register presentation-layer services here.
        services.AddEndpointsApiExplorer();

        return services;
    }
}

