using Microsoft.EntityFrameworkCore;
using TPI_2026.Application.Abstractions.Interfaces;
using TPI_2026.Application.Exceptions;
using TPI_2026.Application.Responses;
using TPI_2026.Domain.Enums;

namespace TPI_2026.Application.Services;

public class MedicalHistoryService(IApplicationDbContext dataBase) : IMedicalHistoryService
{
    public async Task<Guid> AddEntryAsync(
        Guid appointmentId,
        string diagnostic,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(diagnostic))
            throw new ForbiddenException("Diagnostic is required.");

        if (diagnostic.Length > 2000)
            throw new ForbiddenException("Diagnostic cannot exceed 2000 characters.");


        var appointment = await dataBase.Appointments
            .Include(appointment => appointment.MedicalHistory)
            .FirstOrDefaultAsync(appointment => appointment.Id == appointmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Appointment), appointmentId);

        if (appointment.State != AppointmentState.Confirmed)
            throw new ForbiddenException("Medical history can only be added to confirmed appointments.");

        if (appointment.MedicalHistory is not null)
        {
            appointment.MedicalHistory.AddEntry(diagnostic);
            await dataBase.SaveChangesAsync(cancellationToken);
            return appointment.MedicalHistory.Id;
        }


        var history = Domain.Entities.MedicalHistory.Create(
            appointmentId, appointment.PatientId, diagnostic, appointment.DateTime);

        dataBase.MedicalHistories.Add(history);
        await dataBase.SaveChangesAsync(cancellationToken);
        return history.Id;
    }

    public async Task<List<MedicalHistoryDto>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await dataBase.MedicalHistories
            .Where(medicalHistory => medicalHistory.PatientId == patientId)
            .Include(medicalHistory => medicalHistory.Appointment).ThenInclude(appointment => appointment!.Doctor)
            .Select(medicalHistory => new MedicalHistoryDto(
                medicalHistory.Id,
                medicalHistory.AppointmentId,
                medicalHistory.Appointment!.Doctor!.Name,
                medicalHistory.DateTime,
                medicalHistory.Diagnostic,
                medicalHistory.GetSummary()))
            .ToListAsync(cancellationToken);
    }
}