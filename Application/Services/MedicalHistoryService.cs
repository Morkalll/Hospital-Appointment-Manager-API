using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Exceptions;
using TPI_2026.Application.Responses;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;

namespace TPI_2026.Application.Services;

public class MedicalHistoryService(
    IMedicalHistoryRepository medicalHistoryRepo,
    IAppointmentRepository appointmentRepo) : IMedicalHistoryService
{
    public async Task<Guid> CreateMedicalHistoryAsync(
        Guid appointmentId,
        string diagnostic,
        CancellationToken cancellationToken = default)
    {
        if (appointmentId == Guid.Empty)
            throw new ValidationException("AppointmentId is required.");

        if (string.IsNullOrWhiteSpace(diagnostic))
            throw new ValidationException("Diagnostic is required.");

        if (diagnostic.Length > 2000)
            throw new ValidationException("Diagnostic cannot exceed 2000 characters.");

        var appointment = await appointmentRepo.GetWithMedicalHistoryAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), appointmentId);

        if (appointment.State != AppointmentState.Completed)
            throw new ValidationException("Medical history can only be added to completed appointments.");

        if (appointment.MedicalHistory is not null)
            throw new ValidationException("A medical history already exists for this appointment.");

        var newMedicalHistory = new MedicalHistory
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            PatientId = appointment.PatientId,
            Diagnostic = diagnostic,
            DateTime = DateTime.UtcNow
        };

        appointment.MedicalHistory = newMedicalHistory;

        await medicalHistoryRepo.AddAsync(newMedicalHistory, cancellationToken);
        // Note: appointment navigation property modification is saved by the repo Add if tracked, or we can explicit update appointment.
        // EF Core will save the new medical history and link it since we set the AppointmentId.

        return newMedicalHistory.Id;
    }

    public async Task<List<MedicalHistoryDto>> GetPatientMedicalHistoriesAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var medicalHistories = await medicalHistoryRepo.GetByPatientIdWithDetailsAsync(patientId, cancellationToken);

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