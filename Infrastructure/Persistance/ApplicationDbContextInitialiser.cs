using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TPI_2026.Infrastructure.Persistance;

public class ApplicationDbContextInitialiser
(
    ILogger<ApplicationDbContextInitialiser> logger,
    ApplicationDbContext context,
    IPasswordHasher<User> hasher
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
            logger.LogError(exception, "Database migration error.");
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
            logger.LogError(exception, "Initial seeding error.");
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
            };
            admin.Password = hasher.HashPassword(admin, "Admin1234!");

            context.Administrators.Add(admin);
            await context.SaveChangesAsync();
            logger.LogInformation("Initial admin created.");
        }
    }
}
