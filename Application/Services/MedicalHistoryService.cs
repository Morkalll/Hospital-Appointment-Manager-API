using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Exceptions;
using TPI_2026.Application.Responses;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;

namespace TPI_2026.Application.Services;

public class MedicalHistoryService(
    IMedicalHistoryRepository medicalHistoryRepo,
    IAppointmentRepository appointmentRepo,
    ICurrentUserService currentUserService) : IMedicalHistoryService
{
    public async Task<Guid> CreateMedicalHistoryAsync(
        Guid appointmentId,
        string diagnostic,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        if (appointmentId == Guid.Empty)
            errors.Add("AppointmentId is required.");

        if (string.IsNullOrWhiteSpace(diagnostic))
            errors.Add("Diagnostic is required.");
        else if (diagnostic.Length > 2000)
            errors.Add("Diagnostic cannot exceed 2000 characters.");

        if (errors.Count > 0) throw new ValidationException(errors);

        var appointment = await appointmentRepo.GetWithMedicalHistoryAsync(appointmentId, cancellationToken)
            ?? throw new NotFoundException("Appointment");

        var currentUserId = currentUserService.UserId;
        if (currentUserId == null || appointment.DoctorId != currentUserId)
            throw new ForbiddenException("Only the doctor related to the appointment can add a medical history.");

        if (appointment.State != AppointmentState.Completed)
            errors.Add("Medical history can only be added to completed appointments.");

        if (appointment.MedicalHistory is not null)
            errors.Add("A medical history already exists for this appointment.");

        if (errors.Count > 0) throw new ValidationException(errors);

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