using TPI_2026.Domain.Enums;

namespace TPI_2026.Application.Requests
{
    public record CreateDoctorReq(
        string Name,
        string Email,
        string Password,
        string Credential,
        Specialty Specialty
    );
}
