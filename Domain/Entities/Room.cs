using TPI_2026.Domain.Common;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;

namespace TPI_2026.Domain.Entities;

public class Room : BaseEntity
{
    public string Number { get; set; } = string.Empty;
    public int Floor { get; set; }
    public Specialty Specialty { get; set; }


    public Guid? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();
}
