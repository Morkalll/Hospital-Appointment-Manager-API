using TPI_2026.Domain.Common;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;

namespace TPI_2026.Domain.Events;

public class AppointmentChangedEvent(Appointment appointment, AppointmentState previousState) : BaseEvent
{
    public Appointment Appointment { get; } = appointment;
    public AppointmentState AppointmentState { get; } = previousState;
}