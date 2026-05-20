using TPI_2026.Domain.Enums;

namespace TPI_2026.Domain.Entities;

public class Recepcionist : User
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public string Shift { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;

    public Recepcionist() { Role = UserRole.Recepcionist; }
}

