namespace TPI_2026.Application.Requests
{
    public record UpdateDiagnosticReq(
        Guid AppointmentId,
        string Diagnostic
    );
}
