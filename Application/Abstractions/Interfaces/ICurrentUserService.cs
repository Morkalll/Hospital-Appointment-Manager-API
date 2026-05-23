
namespace TPI_2026.Application.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Role { get; }
}