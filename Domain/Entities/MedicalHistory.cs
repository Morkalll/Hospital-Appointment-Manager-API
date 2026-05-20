using TPI_2026.Domain.Common;

namespace TPI_2026.Domain.Entities;


public class MedicalHistory : BaseEntity
{
    public Guid AppointmentId { get; private set; }
    public Guid PatientId { get; private set; }
    public string Diagnostic { get; private set; } = string.Empty;
    public DateTime DateTime { get; private set; }

    // Navigation
    public Appointment? Appointment { get; set; }
    public Patient? Patient { get; set; }

    private MedicalHistory() { }

    public static MedicalHistory Create(Guid appointmentId, Guid patientId, string diagnostic, DateTime dateTime)
    {
        return new MedicalHistory
        {
            AppointmentId = appointmentId,
            PatientId = patientId,
            Diagnostic = diagnostic,
            DateTime = dateTime
        };
    }

    public string GetSummary()
        => $"Turno: {DateTime:dd/MM/yyyy HH:mm} | Diagnóstico: {Diagnostic}";

    public void AddEntry(string diagnostic)
    {
        if (!string.IsNullOrWhiteSpace(diagnostic))
            Diagnostic = diagnostic;
    }
}

