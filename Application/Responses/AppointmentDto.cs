namespace TPI_2026.Application.Responses;

public record AppointmentDto(
    Guid Id,
    Guid DoctorId,
    string DoctorName,
    Guid RoomId,
    string RoomNumber,
    DateTime DateTime,
    string State);

