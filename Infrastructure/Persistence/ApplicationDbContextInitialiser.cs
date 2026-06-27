using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace TPI_2026.Infrastructure.Persistence;

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
            var now = DateTime.UtcNow;
            var admin = new Administrator
            {
                Name = "Admin",
                Email = "admin@hospital.com",
                CreatedAt = now,
                UpdatedAt = now
            };
            admin.Password = BCrypt.Net.BCrypt.HashPassword("Admin1234!");

            context.Administrators.Add(admin);
            await context.SaveChangesAsync();
            logger.LogInformation("Initial admin created.");
        }


        if (!context.Rooms.Any())
        {
            var now = DateTime.UtcNow;
            var rooms = new List<Room>
            {
                new Room { Number = "101", Floor = 1, Specialty = Specialty.Cardiology, CreatedAt = now, UpdatedAt = now },
                new Room { Number = "102", Floor = 1, Specialty = Specialty.Clinic, CreatedAt = now, UpdatedAt = now },
                new Room { Number = "103", Floor = 2, Specialty = Specialty.Pediatrics, CreatedAt = now, UpdatedAt = now },
                new Room { Number = "104", Floor = 2, Specialty = Specialty.Neurology, CreatedAt = now, UpdatedAt = now },
                new Room { Number = "105", Floor = 3, Specialty = Specialty.Traumatology, CreatedAt = now, UpdatedAt = now },
        };

            context.Rooms.AddRange(rooms);
            await context.SaveChangesAsync();
            logger.LogInformation("Initial rooms created.");
        }

        if (!context.Doctors.Any())
        {
            var now = DateTime.UtcNow;
            var doctor = new Doctor
            {
                Name = "Dr. Smith",
                Email = "smith@hospital.com",
                Specialty = Specialty.Cardiology,
                CreatedAt = now,
                UpdatedAt = now
            };
            doctor.Password = BCrypt.Net.BCrypt.HashPassword("Doctor1234!");
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();
            logger.LogInformation("Initial doctor created.");
        }

        if (!context.Appointments.Any())
        {
            var now = DateTime.UtcNow;
            var doctor = context.Doctors.First();
            var room = context.Rooms.First(r => r.Specialty == doctor.Specialty);
            var appointments = new List<Appointment>();

            var startDate = DateTime.UtcNow.Date.AddDays(1);
            for (int day = 0; day < 7; day++)
            {
                var currentDate = startDate.AddDays(day);
                for (int hour = 9; hour <= 19; hour++)
                {
                    appointments.Add(Appointment.CreateAvailable(doctor.Id, room.Id, currentDate.AddHours(hour)));
                    appointments.Add(Appointment.CreateAvailable(doctor.Id, room.Id, currentDate.AddHours(hour).AddMinutes(30)));
                }
            }

            context.Appointments.AddRange(appointments);
            await context.SaveChangesAsync();
            logger.LogInformation("Initial available appointments created.");
        }
    }
}
