using TPI_2026.Application.Abstractions.Interfaces.Events;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Domain.Events;

namespace TPI_2026.Application.Events;

public class SendEmailOnAppointmentCanceledHandler : IEventHandler<AppointmentCanceledEvent>
{
    private readonly IEmailService _emailService;

    public SendEmailOnAppointmentCanceledHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task HandleAsync(AppointmentCanceledEvent domainEvent, CancellationToken cancellationToken)
    {
        var appointment = domainEvent.Appointment;
        var patient = appointment.Patient;
        var doctor = appointment.Doctor;

        var messageDestinatory = patient?.Email;

        if (patient == null || string.IsNullOrEmpty(messageDestinatory)) return;

        var subject = "Cancelación de Turno Médico";
        var body = $"Hola {patient.Name},\n\n" +
                     $"Le informamos que el turno programado para el día {appointment.DateTime:dd/MM/yyyy HH:mm} " +
                     $"con el/la Dr/a. {doctor?.Name} ha sido cancelado.\n\n" +
                     $"Si esto fue un error o desea reprogramar, por favor póngase en contacto con la clínica.\n\n" +
                     $"Saludos,\nAdministración de la Clínica.";

        await _emailService.SendEmailAsync(messageDestinatory, subject, body);
    }
}