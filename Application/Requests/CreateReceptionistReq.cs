namespace TPI_2026.Application.Requests
{
    public record CreateReceptionistReq(
        string Name,
        string Email,
        string Password,
        string EmployeeNumber,
        string WorkingShift,
        string Area
    );
}
