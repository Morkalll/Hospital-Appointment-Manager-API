using TPI_2026.Domain.Enums;

namespace TPI_2026.Domain.Entities;

// Patient hereda de User
public class Patient : User
{
    public string Dni { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public string Adress { get; set; } = string.Empty;

    /* El intefaz ICollection se usa para guardar colecciones de objetos,
    como los turnos y los historiales medicos 
    (revisar la privacidad de metodos get y set)
    */
    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();
    public ICollection<MedicalHistory> MedicalHistories { get; private set; } = new List<MedicalHistory>();

    public Patient() { Role = UserRole.Patient; }
}




