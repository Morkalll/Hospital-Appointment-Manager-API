using TPI_2026.Domain.Common;
using TPI_2026.Domain.Enums;

namespace TPI_2026.Domain.Entities;

public abstract class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; protected set; }

}