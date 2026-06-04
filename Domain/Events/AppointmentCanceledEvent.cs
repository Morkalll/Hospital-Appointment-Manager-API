using TPI_2026.Domain.Common;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;

namespace TPI_2026.Domain.Events;

public class AppointmentCanceledEvent : BaseEvent
{
    public Appointment Appointment { get; }

    public AppointmentCanceledEvent(Appointment appointment)
    {
        Appointment = appointment;
    }
}