using Microsoft.Extensions.DependencyInjection;

namespace TPI_2026.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application-layer services here.
        return services;
    }
}
