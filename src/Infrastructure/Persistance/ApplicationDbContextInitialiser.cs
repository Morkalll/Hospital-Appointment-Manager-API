using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TPI_2026.Infrastructure.Persistance;

public class ApplicationDbContextInitialiser
(
    ILogger<ApplicationDbContextInitialiser> logger,
    ApplicationDbContext context
)

{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error al migrar la base de datos.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error al sembrar datos iniciales.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        if (!context.Administrators.Any())
        {
            var admin = new Administrator
            {
                Name = "Admin",
                Email = "admin@hospital.com",
                Password = "Admin1234!"
            };

            context.Administrators.Add(admin);
            await context.SaveChangesAsync();
            logger.LogInformation("Admin inicial creado.");
        }
    }
}
