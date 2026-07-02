using TPI_2026.Application.Abstractions.Interfaces.Events;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Exceptions;
using TPI_2026.Application.Responses;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;
using TPI_2026.Domain.Exceptions;
using TPI_2026.Domain.Events;

namespace TPI_2026.Application.Services;

public class AppointmentService(
    IAppointmentRepository appointmentRepo,
    IRepository<Patient> patientRepo,
    IRepository<Doctor> doctorRepo,
    IRepository<Room> roomRepo,
    IEventHandler<AppointmentCreatedEvent> createdEventHandler,
    IEventHandler<AppointmentCanceledEvent> canceledEventHandler,
    IEventHandler<AppointmentChangedEvent> changedEventHandler) : IAppointmentService
{
    public async Task<Guid> CreateAsync(Guid patientId, Guid doctorId, Guid roomId, DateTime dateTime, CancellationToken cancellationToken = default)
    {
        if (patientId == Guid.Empty) throw new ValidationException("PatientId is required.");
        if (doctorId == Guid.Empty) throw new ValidationException("DoctorId is required.");
        if (roomId == Guid.Empty) throw new ValidationException("RoomId is required.");
        if (dateTime <= DateTime.UtcNow) throw new ValidationException("Appointment date must be in the future.");

        if (dateTime.Hour < 9 || dateTime.Hour >= 20 || (dateTime.Hour == 20 && dateTime.Minute > 0))
            throw new ValidationException("Appointments can only be scheduled between 09:00 and 20:00.");

        if (dateTime.Minute != 0 && dateTime.Minute != 30)
            throw new ValidationException("Appointments must be scheduled precisely on the hour or half-hour (e.g., 09:00 or 09:30).");

        if (dateTime.Second != 0)
            throw new ValidationException("Appointments must have 0 seconds.");

        var patient = await patientRepo.GetByIdAsync(patientId, cancellationToken)
            ?? throw new NotFoundException("Patient");

        var room = await roomRepo.GetByIdAsync(roomId, cancellationToken)
            ?? throw new NotFoundException("Room");

        var doctor = await doctorRepo.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException("Doctor");

        if (!doctor.IsAvailable)
            throw new ValidationException("The doctor is not available.");

        if (doctor.Specialty != room.Specialty)
            throw new ValidationException("The doctor's specialty does not match with the room's specialty.");

        if (await appointmentRepo.HasDoctorOverlapAsync(doctorId, dateTime, cancellationToken))
            throw new ValidationException("The doctor already has an appointment assigned at that time.");

        if (await appointmentRepo.HasRoomOverlapAsync(roomId, dateTime, cancellationToken))
            throw new ValidationException("The room is already booked for another appointment at that time.");

        var appointment = Appointment.Create(
            patientId,
            doctorId,
            roomId,
            dateTime);

        await appointmentRepo.AddAsync(appointment, cancellationToken);
        
        await createdEventHandler.HandleAsync(new AppointmentCreatedEvent(appointment), cancellationToken);

        return appointment.Id;
    }

    public async Task CancelAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await appointmentRepo.GetByIdAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException("Appointment");

        if (!appointment.IsCancelable())
            throw new NotCancellableAppointmentException(appointmentId);

        appointment.ChangeState(AppointmentState.Canceled);

        await appointmentRepo.UpdateAsync(appointment, cancellationToken);
        
        await canceledEventHandler.HandleAsync(new AppointmentCanceledEvent(appointment), cancellationToken);
    }

    public async Task CompletionAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await appointmentRepo.GetByIdAsync(appointmentId, cancellationToken)
        ?? throw new NotFoundException("Appointment");

        if (!appointment.IsCompleteable())
            throw new NotCompleteableAppointmentException(appointmentId);

        var previousState = appointment.State;
        appointment.ChangeState(AppointmentState.Completed);

        await appointmentRepo.UpdateAsync(appointment, cancellationToken);
        
        await changedEventHandler.HandleAsync(new AppointmentChangedEvent(appointment, previousState), cancellationToken);
    }

    public async Task<List<AppointmentDto>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var appointments = await appointmentRepo.GetByPatientIdAsync(patientId, cancellationToken);

        return appointments
        .Select(appointment => new AppointmentDto(
            appointment.Id,
            appointment.DoctorId,
            appointment.Doctor!.Name,
            appointment.RoomId,
            appointment.Room!.Number,
            appointment.DateTime,
            appointment.State.ToString()))
        .ToList();
    }

    public async Task<List<AppointmentDto>> GetByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        var appointments = await appointmentRepo.GetByDoctorIdAsync(doctorId, cancellationToken);

        return appointments
        .Select(appointment => new AppointmentDto(
            appointment.Id,
            appointment.PatientId,
            appointment.Patient!.Name,
            appointment.RoomId,
            appointment.Room!.Number,
            appointment.DateTime,
            appointment.State.ToString()))
        .ToList();
    }
}

