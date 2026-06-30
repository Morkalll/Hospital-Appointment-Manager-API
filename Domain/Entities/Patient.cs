using TPI_2026.Domain.Enums;

namespace TPI_2026.Domain.Entities;

public class Patient : User
{
    public string Dni { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();
    public ICollection<MedicalHistory> MedicalHistories { get; private set; } = new List<MedicalHistory>();

    public Patient() { Role = UserRole.Patient; }
}
