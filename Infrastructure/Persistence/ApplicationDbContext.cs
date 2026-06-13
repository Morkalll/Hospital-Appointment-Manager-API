using Microsoft.EntityFrameworkCore;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Common;
using TPI_2026.Application.Abstractions.Interfaces.Events;
using Microsoft.Extensions.DependencyInjection; // para usar el BaseEvent

namespace TPI_2026.Infrastructure.Persistence;

// Pase todas las restricciones de cada clase directamente al ApplicationDbContext, asi no esta en la carpeta Configuration 
// que el repo del profe no la tiene, aparte eso se usa para proyectos muy grandes y que el dbcontext no te quede enorme. 
public class ApplicationDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider; // Conoce todas las interfaces, clases y servicios en 'DependencyInjection.cs'
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IServiceProvider serviceProvider)
    : base(options)
    {
        _serviceProvider = serviceProvider;
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


        // numeo de empleado único para que no se repita entre recepcionistas
        modelBuilder.Entity<Receptionist>()
            .HasIndex(r => r.EmployeeNumber)
            .IsUnique();

        modelBuilder.Entity<Patient>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Doctor>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Receptionist>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Administrator>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Room>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Appointment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<MedicalHistory>().HasQueryFilter(e => !e.IsDeleted);
    }




    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        /* Actualiza automáticamente UpdatedAt en toda entidad que haya sido modificada.
        Se hace antes de guardar para que el timestamp refleje el momento exacto
        en que se persistió el cambio. */
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

        /* Se filtran las entidades que heredan de BaseEntity y que tengan algún evento pendiente
           en su lista interna DomainEvents */
        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
        .Where(e => e.Entity.DomainEvents.Any())
        .Select(e => e.Entity)
        .ToList();

        // Unifica en una sola lista los eventos pendientes de diferentes entidades
        var domainEvents = entitiesWithEvents.SelectMany(e => e.DomainEvents).ToList();

        /* Limpia los eventos pendientes en las listas internas de las entidades,
           ya que ahora las almacenamos acá y tenemos que evitar duplicados */
        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        // Se guardan los cambios en la base de datos, y el resultado (int) dentro de 'result' 
        var result = await base.SaveChangesAsync(cancellationToken);

        // Se despacha cada manejador de eventos según los que hay pendientes
        foreach (var domainEvent in domainEvents)
        {
            await DispatcherEventAsync(domainEvent, cancellationToken);
        }

        // Se retorna el total de filas modificadas (ya que este método debe retornar un int)
        return result;
    }


    private async Task DispatcherEventAsync(BaseEvent domainEvent, CancellationToken cancellationToken)
    {
        /* 
        Se accede al "molde" de una clase conocida ('IEventHandler'), 
        y se le asigna como tipo de dato a retornar ('<>') aquel que corresponda 
        a la variable que se le pase por parámetro ('domainEvent'),
        para crear de forma dinámica cierta interfaz, cuyas clases que la implementen 
        serán buscadas más adelante
        */
        var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());

        // Se buscan todas las clases que implementen dicha interfaz 
        var eventHandlers = _serviceProvider.GetServices(eventHandlerType);

        foreach (var eventHandler in eventHandlers)
        {
            // Si no encuentra manejador de evento por algún motivo, pasa al siguiente en la lista
            if (eventHandler is null) continue;

            // Dentro del manejador de eventos encontrado, busca el método que almacena su lógica
            var method = eventHandler.GetType().GetMethod("HandleAsync");
            if (method is not null)
            {
                /*
                Se ejecuta el método. Llamar a 'Invoke' siempre devuelve objetos genéricos,
                por lo que hay que aclararle mediante '(Task)' que lo trate como un asíncrono,
                así podrá aplicarle el 'await' del comienzo
                */
                await (Task)method.Invoke(eventHandler, [domainEvent, cancellationToken])!;
            }
        }
    }
}
