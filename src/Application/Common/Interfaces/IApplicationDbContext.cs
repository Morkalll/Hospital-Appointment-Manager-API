using TPI_2026.Domain.Entities;

namespace TPI_2026.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Patient> Patients { get; }
    DbSet<Doctor> Doctors { get; }
    DbSet<Recepcionist> Recepcionists { get; }
    DbSet<Administrator> Administrators { get; }
    DbSet<Room> Rooms { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<MedicalHistory> MedicalHistories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
