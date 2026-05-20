using TPI_2026.Domain.Common;
using TPI_2026.Domain.Entities;

namespace TPI_2026.Domain.Events;

public class AppointmentCreatedEvent(Appointment appointment) : BaseEvent
{
    public Appointment Appointment { get; } = appointment;
}