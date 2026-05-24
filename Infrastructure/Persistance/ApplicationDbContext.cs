using Microsoft.EntityFrameworkCore;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Common; // para usar el BaseEvent

namespace TPI_2026.Infrastructure.Persistance;

// Pase todas las restricciones de cada clase directamente al ApplicationDbContext, asi no esta en la carpeta Configuration 
// que el repo del profe no la tiene, aparte eso se usa para proyectos muy grandes y que el dbcontext no te quede enorme. 
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Cada DbSet representa una tabla en la base de datos
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

        modelBuilder.Ignore<BaseEvent>(); // Esto es para que no cree una tabla de eventos en la base de datos


        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Patient>("Patient")
            .HasValue<Doctor>("Doctor")
            .HasValue<Receptionist>("Receptionist")
            .HasValue<Administrator>("Administrator");

        // el rol de usuario se guarda como string
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        // email unique para que no se repita entre usuarios
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // dni unique para que no se repita entre pacientes
        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.Dni)
            .IsUnique();

        // crendencial unica para que no se repita entre doctores
        modelBuilder.Entity<Doctor>()
            .HasIndex(d => d.Credential)
            .IsUnique();

        // La especialidad del doctor se guarda como string
        modelBuilder.Entity<Doctor>()
            .Property(d => d.Specialty)
            .HasConversion<string>();

        // Si se borra un doctor, se setea a null el doctor que esta asignado a las sala
        modelBuilder.Entity<Doctor>()
            .HasMany(d => d.Rooms)
            .WithOne(r => r.Doctor)
            .HasForeignKey(r => r.DoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        // La especialidad de la sala se guarda como string
        modelBuilder.Entity<Room>()
            .Property(r => r.Specialty)
            .HasConversion<string>();

        // El estado del turno se guarda como texto en lugar de número
        modelBuilder.Entity<Appointment>()
            .Property(a => a.State)
            .HasConversion<string>();

        // no se puede borrar un paciente si tiene turnos asociados.
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // no se puede borrar un doctor si tiene turnos asociados
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // no se puede borrar una sala si tiene turnos asociados (no se si esto esta bien 
        // ya que a lo mejor se podria cambiar la sala, pero capaz mucho quilombo)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Room)
            .WithMany(r => r.Appointments)
            .HasForeignKey(a => a.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // si se borra un turno, se borra también su historial médico
        modelBuilder.Entity<MedicalHistory>()
            .HasOne(mh => mh.Appointment)
            .WithOne(a => a.MedicalHistory)
            .HasForeignKey<MedicalHistory>(mh => mh.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // si se borra un paciente, se borran también sus historiales médicos 
        // (capaz conviene meter un borrado logico, habria que ver.)
        modelBuilder.Entity<MedicalHistory>()
            .HasOne(mh => mh.Patient)
            .WithMany(p => p.MedicalHistories)
            .HasForeignKey(mh => mh.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}