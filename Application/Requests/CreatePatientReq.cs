namespace TPI_2026.Application.Requests
{
    public record CreatePatientReq(
        string Name,
        string Email,
        string Password,
        string Dni,
        DateOnly BirthDate,
        string PhoneNumber,
        string Address
    );
}
