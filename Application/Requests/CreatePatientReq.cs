namespace TPI_2026.Application.Requests
{
    public record CreatePatientReq(
        string Name,
        string Email,
        string Password,
        string Dni,
        string BirthDate,
        string PhoneNumber,
        string Adress
    );
}
