using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Exceptions;
using TPI_2026.Application.Responses;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;

namespace TPI_2026.Application.Services;

public class MedicalHistoryService(IUnitOfWork unitOfWork) : IMedicalHistoryService
{
    public async Task<Guid> CreateMedicalHistoryAsync(
        Guid appointmentId,
        string diagnostic,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(diagnostic))
            throw new Exception("Diagnostic is required.");

        if (diagnostic.Length > 2000)
            throw new Exception("Diagnostic cannot exceed 2000 characters.");

        var appointment = await unitOfWork.Appointments.GetWithMedicalHistoryAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), appointmentId);

        if (appointment.State != AppointmentState.Confirmed)
            throw new Exception("Medical history can only be added to confirmed appointments.");

        // 1. Si YA TIENE una historia clínica, arrojamos un error para no pisarla
        if (appointment.MedicalHistory is not null)
            throw new Exception("A medical history already exists for this appointment.");

        // 2. Creamos la nueva instancia correctamente asignándola a una variable
        var newMedicalHistory = new MedicalHistory
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            PatientId = appointment.PatientId,
            Diagnostic = diagnostic,
            DateTime = DateTime.UtcNow
        };

        // 3. Lo vinculamos al turno y guardamos en la BD
        appointment.MedicalHistory = newMedicalHistory;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return newMedicalHistory.Id;
    }

    public async Task<List<MedicalHistoryDto>> GetPatientByIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var medicalHistories = await unitOfWork.MedicalHistories.GetByPatientIdWithDetailsAsync(patientId, cancellationToken);

        return medicalHistories
        .Select(medicalHistory => new MedicalHistoryDto(
                medicalHistory.Id,
                medicalHistory.AppointmentId,
                medicalHistory.Appointment!.Doctor!.Name,
                medicalHistory.DateTime,
                medicalHistory.Diagnostic,
                medicalHistory.GetSummary()))
        .ToList();
    }
}