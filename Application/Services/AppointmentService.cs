using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Exceptions;
using TPI_2026.Application.Responses;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;
using TPI_2026.Domain.Exceptions;

namespace TPI_2026.Application.Services;

public class AppointmentService(IUnitOfWork unitOfWork) : IAppointmentService
{
    public async Task<Guid> CreateAsync(Guid patientId, Guid doctorId, Guid roomId, DateTime dateTime, CancellationToken cancellationToken = default)
    {
        if (patientId == Guid.Empty) throw new Exception("PatientId is required.");
        if (doctorId == Guid.Empty) throw new Exception("DoctorId is required.");
        if (roomId == Guid.Empty) throw new Exception("RoomId is required.");
        if (dateTime <= DateTime.UtcNow) throw new Exception("Appointment date must be in the future.");

        var patient = await unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), patientId);

        var room = await unitOfWork.Rooms.GetByIdAsync(roomId, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), roomId);

        var doctor = await unitOfWork.Doctors.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), doctorId);

        if (!doctor.IsAvailable)
            throw new Exception("The doctor is not available.");

        // Validación de si hay solapamiento de horarios con turnos ya existentes.
        var overlaps = await unitOfWork.Appointments.HasOverlapAsync(doctorId, dateTime, cancellationToken);

        if (overlaps)
            throw new Exception("The doctor already has an appointment at that time.");

        // Se asignan las claves foráneas
        var appointment = Appointment.Create(
            patientId,
            doctorId,
            roomId,
            dateTime);

        // Se asignan las propiedades de navegación (para hacer: 'Patient.[]' 'Doctor.[]' 'Room.[]') 
        appointment.Patient = patient;
        appointment.Doctor = doctor;
        appointment.Room = room;

        unitOfWork.Appointments.Add(appointment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return appointment.Id;
    }

    public async Task CancelAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), appointmentId);

        if (!appointment.IsCancelable())
            throw new NotCancellableAppointmentException(appointmentId);

        appointment.ChangeState(AppointmentState.Canceled);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CompletionAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(appointmentId, cancellationToken)
        ?? throw new NotFoundException(nameof(Appointment), appointmentId);

        if (!appointment.IsCompleteable())
            throw new NotCompleteableAppointmentException(appointmentId);

        appointment.ChangeState(AppointmentState.Completed);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<AppointmentDto>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var appointments = await unitOfWork.Appointments.GetByPatientIdAsync(patientId, cancellationToken);

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
}

