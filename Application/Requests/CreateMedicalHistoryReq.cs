namespace TPI_2026.Application.Requests
{
    public record CreateMedicalHistoryReq(
        Guid AppointmentId,
        string Diagnostic
    );
}
