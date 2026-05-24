using TPI_2026.Domain.Enums;

namespace TPI_2026.Application.Abstractions.Interfaces;

public interface IUserService
{
    Task<Guid> CreatePatientAsync(
        string name,
        string email,
        string password,
        string dni,
        string birthDate,
        string phoneNumber,
        string adress,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateDoctorAsync(
        string name,
        string email,
        string password,
        string credential,
        Specialty specialty,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateReceptionistAsync(
        string name,
        string email,
        string password,
        string employeeNumber,
        string workingShift,
        string area,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}