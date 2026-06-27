using TPI_2026.Domain.Enums;

namespace TPI_2026.Domain.Entities;

public class Doctor : User
{
    public string Password { get; set; } = string.Empty;
    public string Credential { get; set; } = string.Empty;
    public Specialty Specialty { get; set; }
    public bool IsAvailable { get; set; } = true;

    public ICollection<Room> Rooms { get; private set; } = new List<Room>();
    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();

    public Doctor() { Role = UserRole.Doctor; }
}
