using TPI_2026.Application.Responses;

namespace TPI_2026.Application.Abstractions.Interfaces.Services;

public interface IAuthService
{
    // Valida las credenciales del usuario y devuelve un token JWT si estan bien
    Task<AuthResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}