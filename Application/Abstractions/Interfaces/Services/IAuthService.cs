using TPI_2026.Application.Responses;

namespace TPI_2026.Application.Abstractions.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}