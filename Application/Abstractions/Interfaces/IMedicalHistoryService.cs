using TPI_2026.Application.Responses;

namespace TPI_2026.Application.Abstractions.Interfaces;

public interface IMedicalHistoryService
{
    Task<Guid> AddEntryAsync(
        Guid appointmentId,
        string diagnostic,
        CancellationToken ct = default);

    Task<List<MedicalHistoryDto>> GetPatientByIdAsync(
        Guid patientId,
        CancellationToken ct = default);
}

