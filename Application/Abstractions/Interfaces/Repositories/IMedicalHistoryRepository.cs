using TPI_2026.Domain.Entities;

namespace TPI_2026.Application.Abstractions.Interfaces.Repositories;

public interface IMedicalHistoryRepository : IRepository<MedicalHistory>
{
    Task<List<MedicalHistory>> GetByPatientIdWithDetailsAsync(Guid patientId, CancellationToken cancellationToken = default);
}