using TPI_2026.Domain.Enums;

namespace TPI_2026.Domain.Entities;

public class Receptionist : User
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public string WorkingShift { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;

    public Receptionist() { Role = UserRole.Receptionist; }
}

