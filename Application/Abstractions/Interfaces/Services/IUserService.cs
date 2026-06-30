using TPI_2026.Domain.Enums;
using TPI_2026.Application.Responses;

namespace TPI_2026.Application.Abstractions.Interfaces.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync(
        CancellationToken ct = default);

    Task<UserDto> GetByIdAsync(
        Guid userId,
        CancellationToken ct = default);

    Task<Guid> RegisterPatientAsync(
        string name,
        string email,
        string dni,
        DateOnly birthDate,
        string phoneNumber,
        string address,
        CancellationToken cancellationToken = default);

    Task<Guid> RegisterDoctorAsync(
        string name,
        string email,
        string password,
        string credential,
        Specialty specialty,
        CancellationToken cancellationToken = default);

    Task<Guid> RegisterReceptionistAsync(
        string name,
        string email,
        string password,
        string employeeNumber,
        string workingShift,
        string area,
        CancellationToken cancellationToken = default);

    Task<Guid> RegisterAdminAsync(
        string name,
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}