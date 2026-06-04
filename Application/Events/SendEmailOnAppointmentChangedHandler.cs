using TPI_2026.Application.Abstractions.Interfaces.Services; 
using TPI_2026.Application.Abstractions.Interfaces.Events;
using TPI_2026.Domain.Events;
using TPI_2026.Domain.Enums;

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
        var previousState = domainEvent.AppointmentState;
        var newState = appointment.State;
        
        var patient = appointment.Patient;
        var doctor = appointment.Doctor;

        // Validación de seguridad idéntica a la que ya usas
        if (patient == null || string.IsNullOrEmpty(patient.Email)) return;

        var subject = "Actualización de Turno Médico";
        var body = $"Hola {patient.Name},\n\n" +
                     $"Le informamos que el estado de su turno programado para el día {appointment.DateTime:dd/MM/yyyy HH:mm} " +
                     $"con el/la Dr/a. {doctor?.Name} ha cambiado.\n" +
                     $"Estado anterior: {previousState}\n" +
                     $"Nuevo estado: {newState}\n\n" +
                     $"Saludos,\nAdministración de la Clínica.";

        await _emailService.SendEmailAsync(patient.Email, subject, body);
    }
}