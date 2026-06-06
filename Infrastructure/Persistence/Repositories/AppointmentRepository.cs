using Microsoft.EntityFrameworkCore;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;

namespace TPI_2026.Infrastructure.Persistence.Repositories;

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(DbContext dbContext) : base(dbContext) { }

    public override async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await ActiveSet
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
            .FirstOrDefaultAsync(appointment => appointment.Id == id, cancellationToken);
    }


    public async Task<Appointment?> GetWithMedicalHistoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await ActiveSet
            .Include(appointment => appointment.MedicalHistory)
            .FirstOrDefaultAsync(appointment => appointment.Id == id, cancellationToken);
    }

    public async Task<List<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await ActiveSet
            .Where(appointment => appointment.PatientId == patientId)
            .Include(appointment => appointment.Doctor)
            .Include(appointment => appointment.Room)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlapAsync(Guid doctorId, DateTime dateTime, CancellationToken cancellationToken = default)
    {
        return await ActiveSet.AnyAsync(appointment =>
            appointment.DoctorId == doctorId
            && appointment.DateTime == dateTime
            && appointment.State != AppointmentState.Canceled,
            cancellationToken);
    }
}