using Microsoft.EntityFrameworkCore;
using TPI_2026.Domain.Entities;


// Este interfaz lo dejo por las dudas pero no creo que lo usemos
namespace TPI_2026.Application.Abstractions.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Patient> Patients { get; }
    DbSet<Doctor> Doctors { get; }
    DbSet<Recepcionist> Recepcionists { get; }
    DbSet<Administrator> Administrators { get; }
    DbSet<Room> Rooms { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<MedicalHistory> MedicalHistories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
