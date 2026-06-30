using Microsoft.EntityFrameworkCore;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Common;
using TPI_2026.Application.Abstractions.Interfaces.Events;
using Microsoft.Extensions.DependencyInjection;

namespace TPI_2026.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IServiceProvider serviceProvider)
    : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Receptionist> Receptionists { get; set; }
    public DbSet<Administrator> Administrators { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<MedicalHistory> MedicalHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<BaseEvent>();


        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Patient>("Patient")
            .HasValue<Doctor>("Doctor")
            .HasValue<Receptionist>("Receptionist")
            .HasValue<Administrator>("Administrator");

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.Dni)
            .IsUnique()
            .HasFilter("[Dni] IS NOT NULL AND [IsDeleted] = 0");

        modelBuilder.Entity<Doctor>()
            .HasIndex(d => d.Credential)
            .IsUnique()
            .HasFilter("[Credential] IS NOT NULL AND [IsDeleted] = 0");

        modelBuilder.Entity<Doctor>()
            .Property(d => d.Specialty)
            .HasConversion<string>();

        modelBuilder.Entity<Doctor>()
            .HasMany(d => d.Rooms)
            .WithOne(r => r.Doctor)
            .HasForeignKey(r => r.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Room>()
            .Property(r => r.Specialty)
            .HasConversion<string>();

        modelBuilder.Entity<Appointment>()
            .Property(a => a.State)
            .HasConversion<string>();

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Room)
            .WithMany(r => r.Appointments)
            .HasForeignKey(a => a.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MedicalHistory>()
            .HasOne(mh => mh.Appointment)
            .WithOne(a => a.MedicalHistory)
            .HasForeignKey<MedicalHistory>(mh => mh.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MedicalHistory>()
            .HasOne(mh => mh.Patient)
            .WithMany(p => p.MedicalHistories)
            .HasForeignKey(mh => mh.PatientId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Receptionist>()
            .HasIndex(r => r.EmployeeNumber)
            .IsUnique()
            .HasFilter("[EmployeeNumber] IS NOT NULL AND [IsDeleted] = 0");

        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Room>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Appointment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<MedicalHistory>().HasQueryFilter(e => !e.IsDeleted);
    }



    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = now;
        }

        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
        .Where(e => e.Entity.DomainEvents.Any())
        .Select(e => e.Entity)
        .ToList();

        var domainEvents = entitiesWithEvents.SelectMany(e => e.DomainEvents).ToList();

        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await DispatcherEventAsync(domainEvent, cancellationToken);
        }

        return result;
    }


    private async Task DispatcherEventAsync(BaseEvent domainEvent, CancellationToken cancellationToken)
    {
        var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());

        var eventHandlers = _serviceProvider.GetServices(eventHandlerType);

        foreach (var eventHandler in eventHandlers)
        {
            if (eventHandler is null) continue;

            var method = eventHandler.GetType().GetMethod("HandleAsync");
            if (method is not null)
            {
                await (Task)method.Invoke(eventHandler, [domainEvent, cancellationToken])!;
            }
        }
    }
}
