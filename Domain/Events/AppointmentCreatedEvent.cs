using TPI_2026.Domain.Common;
using TPI_2026.Domain.Entities;

namespace TPI_2026.Domain.Events;

public class AppointmentCreatedEvent : BaseEvent
{
    public Appointment Appointment { get; }

    public AppointmentCreatedEvent(Appointment appointment)
    {
        Appointment = appointment;
    }
}