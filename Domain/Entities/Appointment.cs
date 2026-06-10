
using TPI_2026.Domain.Common;
using TPI_2026.Domain.Enums;
using TPI_2026.Domain.Events;

namespace TPI_2026.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid RoomId { get; private set; }
    public DateTime DateTime { get; private set; }
    public AppointmentState State { get; private set; } = AppointmentState.Confirmed;

    // Navigation
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
    public Room? Room { get; set; }
    public MedicalHistory? MedicalHistory { get; set; }

    private Appointment() { }

    public static Appointment Create(Guid patientId, Guid doctorId, Guid roomId, DateTime dateTime)
    {
        var appointment = new Appointment
        {
            PatientId = patientId,
            DoctorId = doctorId,
            RoomId = roomId,
            DateTime = dateTime,
            State = AppointmentState.Confirmed
        };

        appointment.AddDomainEvent(new AppointmentCreatedEvent(appointment));
        return appointment;
    }

    public void ChangeState(AppointmentState newState)
    {
        var previousState = State;
        State = newState;

        AddDomainEvent(new AppointmentChangedEvent(this, previousState));

        if (newState == AppointmentState.Canceled)
            AddDomainEvent(new AppointmentCanceledEvent(this));
    }

    public bool IsCancelable()
        => State is AppointmentState.Confirmed;

    public bool IsCompleteable()
    => State is AppointmentState.Confirmed;
}