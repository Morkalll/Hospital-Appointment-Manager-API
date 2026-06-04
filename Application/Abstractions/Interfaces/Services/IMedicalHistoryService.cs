using TPI_2026.Application.Responses;

namespace TPI_2026.Application.Abstractions.Interfaces.Services;

public interface IMedicalHistoryService
{
    Task<Guid> CreateMedicalHistoryAsync(
        Guid appointmentId,
        string diagnostic,
        CancellationToken ct = default);

    Task<List<MedicalHistoryDto>> GetPatientByIdAsync(
        Guid patientId,
        CancellationToken ct = default);
}

