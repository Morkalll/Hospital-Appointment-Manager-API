using TPI_2026.Domain.Entities;

namespace TPI_2026.Application.Abstractions.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IAppointmentRepository Appointments { get; }
    IMedicalHistoryRepository MedicalHistories { get; }
    IRepository<Patient> Patients { get; }
    IRepository<Doctor> Doctors { get; }
    IRepository<Receptionist> Receptionists { get; }
    IRepository<Administrator> Administrators { get; }
    IRepository<Room> Rooms { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}