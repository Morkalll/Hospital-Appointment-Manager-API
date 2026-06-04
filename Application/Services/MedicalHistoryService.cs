using Microsoft.EntityFrameworkCore;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Exceptions;
using TPI_2026.Application.Responses;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;

namespace TPI_2026.Application.Services;

public class MedicalHistoryService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IMedicalHistoryService
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
            
        var newMedicalHistory = new MedicalHistory
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            Diagnostic = diagnostic,
            DateTime = DateTime.UtcNow
        };
        appointment.MedicalHistory = newMedicalHistory;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return newMedicalHistory.Id;

    }

    public async Task<List<MedicalHistoryDto>> GetPatientByIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        if (currentUserService.Role == "Patient" && currentUserService.UserId != patientId)
            throw new ForbiddenException("Patients can only access their own medical history.");

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