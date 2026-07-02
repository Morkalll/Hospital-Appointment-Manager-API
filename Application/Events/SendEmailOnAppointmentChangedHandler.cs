using TPI_2026.Application.Abstractions.Interfaces.Events;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Domain.Enums;
using TPI_2026.Domain.Events;

namespace TPI_2026.Application.Events;

public class SendEmailOnAppointmentChangedHandler : IEventHandler<AppointmentChangedEvent>
{
    private readonly IEmailService _emailService;

    public SendEmailOnAppointmentChangedHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task HandleAsync(AppointmentChangedEvent domainEvent, CancellationToken cancellationToken)
    {
        var appointment = domainEvent.Appointment;
        var patient = appointment.Patient;
        
        var messageDestinatory = patient?.Email;

        if (patient == null || string.IsNullOrEmpty(messageDestinatory)) return;

        if (domainEvent.PreviousState == AppointmentState.Confirmed && appointment.State == AppointmentState.Completed)
        {
            var subject = "Su Turno Médico ha Finalizado";
            var body = $"Hola {patient.Name},\n\n" +
                     $"Le informamos que su turno del día {appointment.DateTime:dd/MM/yyyy HH:mm} ha finalizado exitosamente.\n\n" +
                     $"Gracias por confiar en nuestra clínica.";

            await _emailService.SendEmailAsync(messageDestinatory, subject, body);
        }
    }
}
