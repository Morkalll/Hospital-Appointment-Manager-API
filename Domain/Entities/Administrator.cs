using TPI_2026.Domain.Enums;

namespace TPI_2026.Domain.Entities;

public class Administrator : User
{
    public string Password { get; set; } = string.Empty;
    public Administrator() { Role = UserRole.Administrator; }
}