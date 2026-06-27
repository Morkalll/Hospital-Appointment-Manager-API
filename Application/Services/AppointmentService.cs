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
        if (patientId == Guid.Empty) throw new ValidationException("PatientId is required.");
        if (doctorId == Guid.Empty) throw new ValidationException("DoctorId is required.");
        if (roomId == Guid.Empty) throw new ValidationException("RoomId is required.");
        if (dateTime <= DateTime.UtcNow) throw new ValidationException("Appointment date must be in the future.");

        var patient = await unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), patientId);

        var room = await unitOfWork.Rooms.GetByIdAsync(roomId, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), roomId);

        var doctor = await unitOfWork.Doctors.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), doctorId);

        if (!doctor.IsAvailable)
            throw new ValidationException("The doctor is not available.");

        if (doctor.Specialty != room.Specialty)
            throw new ValidationException("The doctor's specialty does not match with the room's specialty.");

        var appointment = await unitOfWork.Appointments.GetAvailableAsync(doctorId, dateTime, cancellationToken);
        if (appointment == null)
            throw new ValidationException("There is no available appointment for the selected doctor at that time.");

        if (appointment.RoomId != roomId)
            throw new ValidationException("The selected room does not match the available appointment.");

        appointment.AssignPatient(patientId);

        // Se asignan las propiedades de navegación
        appointment.Patient = patient;
        appointment.Doctor = doctor;

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

