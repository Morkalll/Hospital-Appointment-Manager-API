
namespace TPI_2026.Application.Abstractions.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(
        string messageDestinatory,
        string messageSubject,
        string messageBody,
        CancellationToken cancellationToken = default);
}