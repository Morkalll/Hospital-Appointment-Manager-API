using TPI_2026.Domain.Enums;

namespace TPI_2026.Application.Responses;

public record RoomDto(
    Guid Id,
    string Number,
    int Floor,
    Specialty Specialty,
    Guid? DoctorId
);
