using TPI_2026.Domain.Enums;

namespace TPI_2026.Domain.Entities;

// Doctor hereda de User
public class Doctor : User
{
    public string Password { get; set; } = string.Empty;
    public string Credential { get; set; } = string.Empty;
    // Atributo Specialty es del tipo enum Specialty
    public Specialty Specialty { get; set; }
    public bool IsAvailable { get; set; } = true;
    public UserRole Role { get; protected set; }


    //Cardinalidades
    public ICollection<Room> Rooms { get; private set; } = new List<Room>();
    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();

    public Doctor() { Role = UserRole.Doctor; }
}



