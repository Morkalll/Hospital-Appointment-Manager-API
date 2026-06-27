using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using TPI_2026.Application.Abstractions.Interfaces.Services;

namespace TPI_2026.Infrastructure.Persistence.Services;

public class EmailService : IEmailService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public EmailService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string messageDestinatory,
        string messageSubject,
        string messageBody,
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("ResendClient");

        var emailPayload = new
        {
            from = _configuration["EmailSettings:From"] ?? "onboarding@resend.dev",
            to = new[] { messageDestinatory },
            subject = messageSubject,
            html = messageBody
        };

        var response = await client.PostAsJsonAsync("emails", emailPayload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Fallo al enviar el correo: {errorResponse}");
        }
    }
}