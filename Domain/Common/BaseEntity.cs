namespace TPI_2026.Domain.Common;

public abstract class BaseEntity
{
    // Declaración de la variable Id de tipo Guid, con su método get, y el accesor "init"
    // que reemplaza al set, para que solo sea asignada durante la inicialización del objeto.
    public Guid Id { get; init; } = Guid.NewGuid();

    // Declaración de la lista _domainEvents (uso privado), que retorna elementos
    // de tipo BaseEvent (clase abstracta declarada en la misma carpeta que esta).
    private readonly List<BaseEvent> _domainEvents = new List<BaseEvent>();

    // Declaración de la versión pública de _domainEvents, colección a la que
    // se le asignan gets de solo lectura de la lista mencionada previamente
    // (usando "=>" se abrevia esto último)
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();


    public void AddDomainEvent(BaseEvent e) => _domainEvents.Add(e);
    public void RemoveDomainEvent(BaseEvent e) => _domainEvents.Remove(e);
    public void ClearDomainEvents() => _domainEvents.Clear();

    // IsDeleted para manejar borrado logico, y lo demas para 
    // manejar fechas y tiempos de create, update y delete
    public bool IsDeleted {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
    public DateTime? DeletedAt {get; set;}
   
}  