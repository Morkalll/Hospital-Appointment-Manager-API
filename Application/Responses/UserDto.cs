using TPI_2026.Domain.Enums;

namespace TPI_2026.Application.Responses;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string Role
    );

public record PatientDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Dni,
    DateOnly BirthDate,
    string PhoneNumber,
    string Address
    ) : UserDto(Id, Name, Email, Role);

public record DoctorDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Credential,
    Specialty Specialty,
    bool IsAvailable
    ) : UserDto(Id, Name, Email, Role);

public record ReceptionistDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string EmployeeNumber,
    string WorkingShift,
    string Area
    ) : UserDto(Id, Name, Email, Role);

