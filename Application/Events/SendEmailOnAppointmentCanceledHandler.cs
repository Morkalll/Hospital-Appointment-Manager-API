using TPI_2026.Application.Abstractions.Interfaces.Events;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Domain.Events;
using TPI_2026.Domain.Enums;

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

        if (patient == null || string.IsNullOrEmpty(patient.Email)) return;

        var quienCancelo = domainEvent.CanceledByState == AppointmentState.CanceledByDoctor
            ? $"el/la Dr/a. {doctor?.Name}"
            : "usted";

        var asunto = "Cancelación de Turno Médico";
        var cuerpo = $"Hola {patient.Name},\n\n" +
                     $"Le informamos que el turno programado para el día {appointment.DateTime:dd/MM/yyyy HH:mm} " +
                     $"con el/la Dr/a. {doctor?.Name} ha sido cancelado por {quienCancelo}.\n\n" +
                     $"Si esto fue un error o desea reprogramar, por favor póngase en contacto con la clínica.\n\n" +
                     $"Saludos,\nAdministración de la Clínica.";

        await _emailService.SendEmailAsync(patient.Email, asunto, cuerpo);
    }
}