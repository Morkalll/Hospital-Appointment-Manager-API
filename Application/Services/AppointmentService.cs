using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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

        var appointment = Appointment.Create(patientId, doctorId, roomId, dateTime);
        unitOfWork.Appointments.Add(appointment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return appointment.Id;
    }

    public async Task CancelAsync(Guid appointmentId, bool isDoctor, CancellationToken cancellationToken = default)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), appointmentId);

        if (!appointment.IsCancelable())
            throw new NotCancellableAppointmentException(appointmentId);

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

