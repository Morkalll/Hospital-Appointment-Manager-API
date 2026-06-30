using TPI_2026.Domain.Entities;

namespace TPI_2026.Application.Abstractions.Interfaces.Repositories;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<Appointment?> GetWithMedicalHistoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<List<Appointment>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
    Task<bool> HasDoctorOverlapAsync(Guid doctorId, DateTime dateTime, CancellationToken cancellationToken = default);
    Task<bool> HasRoomOverlapAsync(Guid roomId, DateTime dateTime, CancellationToken cancellationToken = default);
}