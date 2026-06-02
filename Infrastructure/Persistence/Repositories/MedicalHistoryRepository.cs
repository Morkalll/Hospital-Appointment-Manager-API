using Microsoft.EntityFrameworkCore;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Domain.Entities;

namespace TPI_2026.Infrastructure.Persistence.Repositories;

public class MedicalHistoryRepository : Repository<MedicalHistory>, IMedicalHistoryRepository
{
    public MedicalHistoryRepository(DbContext dbContext) : base(dbContext) { }

    public async Task<List<MedicalHistory>> GetByPatientIdWithDetailsAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(medicalHistory => medicalHistory.PatientId == patientId)
            .Include(medicalHistory => medicalHistory.Appointment)
                .ThenInclude(appointment => appointment!.Doctor)
            .ToListAsync(cancellationToken);
    }
}