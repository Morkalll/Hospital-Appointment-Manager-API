namespace TPI_2026.Application.Responses;

public record MedicalHistoryDto(
    Guid Id,
    Guid AppointmentId,
    string DoctorName,
    DateTime DateTime,
    string Diagnostic,
    string Summary);


