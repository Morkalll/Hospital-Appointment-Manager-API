using Microsoft.EntityFrameworkCore;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Domain.Entities;

namespace TPI_2026.Infrastructure.Persistance.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;

    private IAppointmentRepository? _appointments;
    private IMedicalHistoryRepository? _medicalHistories;
    private IRepository<Patient>? _patients;
    private IRepository<Doctor>? _doctors;
    private IRepository<Receptionist>? _receptionists;
    private IRepository<Administrator>? _administrators;
    private IRepository<Room>? _rooms;

    public UnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAppointmentRepository Appointments =>
        _appointments ??= new AppointmentRepository(_dbContext);

    public IMedicalHistoryRepository MedicalHistories =>
        _medicalHistories ??= new MedicalHistoryRepository(_dbContext);

    public IRepository<Patient> Patients =>
        _patients ??= new Repository<Patient>(_dbContext);

    public IRepository<Doctor> Doctors =>
        _doctors ??= new Repository<Doctor>(_dbContext);

    public IRepository<Receptionist> Receptionists =>
        _receptionists ??= new Repository<Receptionist>(_dbContext);

    public IRepository<Administrator> Administrators =>
        _administrators ??= new Repository<Administrator>(_dbContext);

    public IRepository<Room> Rooms =>
        _rooms ??= new Repository<Room>(_dbContext);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}