
using TPI_2026.Domain.Common;
using TPI_2026.Domain.Enums;
using TPI_2026.Domain.Events;

namespace TPI_2026.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid? PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid RoomId { get; private set; }
    public DateTime DateTime { get; private set; }
    public AppointmentState State { get; private set; } = AppointmentState.Available;

    // Navigation
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
    public Room? Room { get; set; }
    public MedicalHistory? MedicalHistory { get; set; }

    private Appointment() { }

    public static Appointment CreateAvailable(Guid doctorId, Guid roomId, DateTime dateTime)
    {
        var appointment = new Appointment
        {
            PatientId = null,
            DoctorId = doctorId,
            RoomId = roomId,
            DateTime = dateTime,
            State = AppointmentState.Available
        };

        return appointment;
    }

    public void AssignPatient(Guid patientId)
    {
        PatientId = patientId;
        ChangeState(AppointmentState.Confirmed);
        AddDomainEvent(new AppointmentCreatedEvent(this));
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