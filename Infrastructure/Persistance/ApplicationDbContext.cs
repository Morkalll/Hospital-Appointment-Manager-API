using System.Reflection;
using TPI_2026.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TPI_2026.Domain.Enums;
using TPI_2026.Application.Abstractions.Interfaces;


namespace TPI_2026.Infrastructure.Persistance;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
   : DbContext(options), IApplicationDbContext
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Recepcionist> Recepcionists => Set<Recepcionist>();
    public DbSet<Administrator> Administrators => Set<Administrator>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<MedicalHistory> MedicalHistories => Set<MedicalHistory>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Ignore<Domain.Common.BaseEvent>();


        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var result = await base.SaveChangesAsync(ct);
        return result;
    }



}