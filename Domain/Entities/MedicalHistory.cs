using TPI_2026.Domain.Common;

namespace TPI_2026.Domain.Entities;


public class MedicalHistory : BaseEntity
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public string Diagnostic { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }

    // Navigation
    public Appointment? Appointment { get; set; }
    public Patient? Patient { get; set; }

    public MedicalHistory() { }

    public string GetSummary()
        => $"Turno: {DateTime:dd/MM/yyyy HH:mm} | Diagnóstico: {Diagnostic}";

    public void CreateMedicalHistoryAsync(string diagnostic)
    {
        throw new NotImplementedException();
    }
}

