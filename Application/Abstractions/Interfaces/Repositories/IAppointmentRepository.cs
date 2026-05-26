using TPI_2026.Domain.Entities;

namespace TPI_2026.Application.Abstractions.Interfaces.Repositories;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<Appointment?> GetWithMedicalHistoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<bool> HasOverlapAsync(Guid doctorId, DateTime dateTime, CancellationToken cancellationToken = default);
}