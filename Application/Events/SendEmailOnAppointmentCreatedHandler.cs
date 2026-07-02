using TPI_2026.Application.Abstractions.Interfaces.Events;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Domain.Events;

namespace TPI_2026.Application.Events;

public class SendEmailOnAppointmentCreatedHandler : IEventHandler<AppointmentCreatedEvent>
{
    private readonly IEmailService _emailService;

    public SendEmailOnAppointmentCreatedHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task HandleAsync(AppointmentCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        var appointment = domainEvent.Appointment;
        var patient = appointment.Patient;
        var doctor = appointment.Doctor;
        var room = appointment.Room;

        var messageDestinatory = patient?.Email;

        if (patient == null || string.IsNullOrEmpty(messageDestinatory)) return;

        var subject = "Confirmación de Turno Médico";
        var body = $"Hola {patient.Name},\n\n" +
                 $"Tu turno ha sido registrado para el día {appointment.DateTime:dd/MM/yyyy HH:mm}.\n" +
                 $"Médico: Dr/a. {doctor?.Name}\n" +
                 $"Sala N°: {room?.Number}\n\n" +
                 $"Saludos,\nAdministración de la Clínica.";

        await _emailService.SendEmailAsync(messageDestinatory, subject, body);
    }
}