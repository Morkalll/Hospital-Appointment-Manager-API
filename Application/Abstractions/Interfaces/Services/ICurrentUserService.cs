
namespace TPI_2026.Application.Abstractions.Interfaces.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Role { get; }
}