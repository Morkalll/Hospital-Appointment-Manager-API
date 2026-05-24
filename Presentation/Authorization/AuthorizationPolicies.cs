using Microsoft.Extensions.DependencyInjection;

namespace TPI_2026.Presentation.Authorization;

public static class AuthorizationPolicies
{
    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("PatientOnly", policy => policy.RequireRole("Patient"));
            options.AddPolicy("DoctorOnly", policy => policy.RequireRole("Doctor"));
            options.AddPolicy("ReceptionistOnly", policy => policy.RequireRole("Receptionist"));
            options.AddPolicy("AdministratorOnly", policy => policy.RequireRole("Administrator"));
            options.AddPolicy("Staff", policy => policy.RequireRole("Receptionist", "Administrator"));
        });
    }
}
