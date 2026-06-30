namespace TPI_2026.Application.Requests
{
    public record CreateAdminReq(
        string Name,
        string Email,
        string Password
    );
}
