using Microsoft.EntityFrameworkCore;
using TPI_2026.Application.Abstractions.Interfaces;
using TPI_2026.Application.Exceptions;
using TPI_2026.Application.Responses;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;
using TPI_2026.Domain.Exceptions;

namespace TPI_2026.Application.Services;

public class AppointmentService(IApplicationDbContext database) : IAppointmentService
{
    public async Task<Guid> CreateAsync(Guid patientId, Guid doctorId, Guid roomId, DateTime dateTime, CancellationToken cancellationToken = default)
    {
        if (patientId == Guid.Empty) throw new ForbiddenException("PatientId is required.");
        if (doctorId == Guid.Empty) throw new ForbiddenException("DoctorId is required.");
        if (roomId == Guid.Empty) throw new ForbiddenException("RoomId is required.");
        if (dateTime <= DateTime.UtcNow) throw new ForbiddenException("Appointment date must be in the future.");

        var doctor = await database.Doctors.FirstOrDefaultAsync(doctor => doctor.Id == doctorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), doctorId);

        if (!doctor.IsAvailable)
            throw new ForbiddenException("The doctor is not available.");

        _ = await database.Patients.FirstOrDefaultAsync(patient => patient.Id == patientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), patientId);

        _ = await database.Rooms.FirstOrDefaultAsync(room => room.Id == roomId, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), roomId);

        // Validación de si hay solapamiento de horarios con turnos ya existentes.
        var overlaps = await database.Appointments.AnyAsync(appointment =>
            appointment.DoctorId == doctorId
            && appointment.DateTime == dateTime
            && appointment.State != AppointmentState.CanceledByDoctor
            && appointment.State != AppointmentState.CanceledByPatient, cancellationToken);

        if (overlaps)
            throw new ForbiddenException("The doctor already has an appointment at that time.");

        var appointment = Appointment.Create(patientId, doctorId, roomId, dateTime);
        database.Appointments.Add(appointment);
        await database.SaveChangesAsync(cancellationToken);
        return appointment.Id;
    }

    public async Task CancelAsync(Guid appointmentId, bool isDoctor, CancellationToken cancellationToken = default)
    {
        var appointment = await database.Appointments.FirstOrDefaultAsync(appointment => appointment.Id == appointmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), appointmentId);

        if (!appointment.IsCancelable())
            throw new NotCancellableAppointmentException(appointmentId);

        appointment.ChangeState(isDoctor
            ? AppointmentState.CanceledByDoctor
            : AppointmentState.CanceledByPatient);

        await database.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await database.Appointments.FirstOrDefaultAsync(appointment => appointment.Id == appointmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), appointmentId);

        if (appointment.State != AppointmentState.Pending)
            throw new ForbiddenException("Only pending appointments can be approved.");

        appointment.ChangeState(AppointmentState.Confirmed);
        await database.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<AppointmentDto>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await database.Appointments
        .Where(appointment => appointment.PatientId == patientId)
        .Include(appointment => appointment.Doctor)
        .Include(appointment => appointment.Room)
        .Select(appointment => new AppointmentDto(
            appointment.Id,
            appointment.DoctorId,
            appointment.Doctor!.Name,
            appointment.RoomId,
            appointment.Room!.Number,
            appointment.DateTime,
            appointment.State.ToString()))
        .ToListAsync(cancellationToken);
    }
}

