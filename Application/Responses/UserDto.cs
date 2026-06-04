namespace TPI_2026.Application.Responses;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string Role
    );
