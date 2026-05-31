using Microsoft.EntityFrameworkCore;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Exceptions;
using TPI_2026.Application.Responses;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;
using TPI_2026.Domain.Events;
using TPI_2026.Domain.Exceptions;

namespace TPI_2026.Application.Services;

public class AppointmentService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IAppointmentService
{
    public async Task<Guid> CreateAsync(Guid patientId, Guid doctorId, Guid roomId, DateTime dateTime, CancellationToken cancellationToken = default)
    {
        if (patientId == Guid.Empty) throw new ForbiddenException("PatientId is required.");
        if (doctorId == Guid.Empty) throw new ForbiddenException("DoctorId is required.");
        if (roomId == Guid.Empty) throw new ForbiddenException("RoomId is required.");
        if (dateTime <= DateTime.UtcNow) throw new ForbiddenException("Appointment date must be in the future.");

        var doctor = await unitOfWork.Doctors.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), doctorId);

        if (!doctor.IsAvailable)
            throw new ForbiddenException("The doctor is not available.");

        _ = await unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), patientId);

        _ = await unitOfWork.Rooms.GetByIdAsync(roomId, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), roomId);

        // Validación de si hay solapamiento de horarios con turnos ya existentes.
        var overlaps = await unitOfWork.Appointments.AnyAsync(appointment =>
            appointment.DoctorId == doctorId
            && appointment.DateTime == dateTime
            && appointment.State != AppointmentState.CanceledByDoctor
            && appointment.State != AppointmentState.CanceledByPatient, cancellationToken);

        if (overlaps)
            throw new ForbiddenException("The doctor already has an appointment at that time.");

        // Se asignan las claves foráneas
        var appointment = Appointment.Create(
            patientId,
            doctorId,
            roomId,
            dateTime);

        // Se asignan las propiedades de navegación (para hacer: 'Patient.[]' 'Doctor.[]' 'Room.[]') 
        var patientEntity = await unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken);
        appointment.Patient = patientEntity;
        appointment.Doctor = doctor; // Traído en línea 23
        appointment.Room = await unitOfWork.Rooms.GetByIdAsync(roomId, cancellationToken);

        appointment.AddDomainEvent(new AppointmentCreatedEvent(appointment));

        unitOfWork.Appointments.Add(appointment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return appointment.Id;
    }

    public async Task CancelAsync(Guid appointmentId, bool isDoctor, CancellationToken cancellationToken = default)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), appointmentId);

        if (currentUserService.Role == "Patient" && appointment.PatientId != currentUserService.UserId)
            throw new ForbiddenException("A patient can only cancel their own appointments.");

        if (currentUserService.Role == "Doctor" && appointment.DoctorId != currentUserService.UserId)
            throw new ForbiddenException("A doctor can only cancel their own appointments.");

        if (!appointment.IsCancelable())
            throw new NotCancellableAppointmentException(appointmentId);

        appointment.Patient = await unitOfWork.Patients.GetByIdAsync(appointment.PatientId, cancellationToken);
        appointment.Doctor = await unitOfWork.Doctors.GetByIdAsync(appointment.DoctorId, cancellationToken);

        appointment.ChangeState(isDoctor
            ? AppointmentState.CanceledByDoctor
            : AppointmentState.CanceledByPatient);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), appointmentId);

        if (appointment.State != AppointmentState.Pending)
            throw new ForbiddenException("Only pending appointments can be approved.");

        appointment.ChangeState(AppointmentState.Confirmed);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<AppointmentDto>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        // Validacon para que los pacientes solo puedan ver sus propios turnos
        if (currentUserService.Role == "Patient" && currentUserService.UserId != patientId)
            throw new ForbiddenException("A patient can only view their own appointments.");
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

