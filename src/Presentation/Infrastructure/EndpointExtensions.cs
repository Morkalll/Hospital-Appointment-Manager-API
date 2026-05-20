using System.Reflection;

namespace TPI_2026.Presentation.Infrastructure;

public static class EndpointExtensions
{
    public static void MapEndpoints(this WebApplication app, Assembly assembly)
    {
        var endpointTypes = assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IEndpoint).IsAssignableFrom(t));

        foreach (var type in endpointTypes)
        {
            var endpoint = (IEndpoint)Activator.CreateInstance(type)!;
            endpoint.MapEndpoint(app);
        }
    }
}