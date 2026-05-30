using MimeKit; // El armador de cartas
using MailKit.Net.Smtp; // El cartero
using Microsoft.Extensions.Configuration;
using TPI_2026.Application.Abstractions.Interfaces.Services;

namespace TPI_2026.Infrastructure.Persistance.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration; // Lee credenciales y parámetros del servidor desde 'appsettings.json'


    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string messageDestinatary,
        string messageSubject,
        string messageBody,
        CancellationToken cancellationToken = default)
    {
        // Crea un objeto carta vacío
        var email = new MimeMessage();

        // Define el remitente. Recibe el correo como texto plano y lo parsea a un formato estricto
        email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:From"] // Lee desde 'appsettings.json'
        ?? throw new InvalidOperationException("La configuración 'EmailSettings:From' es obligatoria.")));

        // Define el destinatario. Misma lógica que el método anterior
        email.To.Add(MailboxAddress.Parse(messageDestinatary));

        // Define el asunto del mensaje
        email.Subject = messageSubject;


        // Formatea el cuerpo del mensaje como texto HTML
        var bodyBuilder = new BodyBuilder { HtmlBody = messageBody };

        // Acopla lo generado previamente como cuerpo final del mensaje
        email.Body = bodyBuilder.ToMessageBody();


        /* 
        Se instancia el cliente de red (el 'using' garantiza que el objeto se destruya
        tras su uso, ya sea por éxito o por fallo -> excepción, evitando fugas de memoria) 
        */
        using var smtp = new SmtpClient(); // Simple Mail Transfer Protocol


        // Abre canal de comunicación al servidor de Google
        await smtp.ConnectAsync(

        _configuration["EmailSettings:Host"] // Lee desde 'appsettings.json'
        ?? throw new InvalidOperationException("La configuración 'EmailSettings:Host' es obligatoria."),

        int.Parse(_configuration["EmailSettings:Port"] ?? "587"), // Lee desde 'appsettings.json'

        MailKit.Security.SecureSocketOptions.StartTls, // Medida de seguridad insana

        cancellationToken);


        // Inicia sesión en el servidor del proveedor
        await smtp.AuthenticateAsync(

        _configuration["EmailSettings:Username"]
        ?? throw new InvalidOperationException("La configuración 'EmailSettings:Username' es obligatoria."),

        _configuration["EmailSettings:Password"]
        ?? throw new InvalidOperationException("La configuración 'EmailSettings:Password' es obligatoria."),

        cancellationToken);


        // Envía el mensaje
        await smtp.SendAsync(email, cancellationToken);

        // Se desconecta del servidor (el 'true' avisa que es una acción limpia y voluntaria)
        await smtp.DisconnectAsync(true, cancellationToken);
    }
}