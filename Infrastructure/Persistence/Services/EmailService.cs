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

        // Arma el objeto JSON según la documentación de Resend
        var emailPayload = new
        {
            from = _configuration["EmailSettings:From"] ?? "onboarding@resend.dev",
            to = new[] { messageDestinatory }, //Crea un array y agrego el string 
            subject = messageSubject,
            html = messageBody
        };

        // Envía la petición POST al endpoint de Resend
        var response = await client.PostAsJsonAsync("emails", emailPayload, cancellationToken);

        // Lanza una excepción si la respuesta no es exitosa 
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Fallo al enviar el correo: {errorResponse}");
        }
    }
}