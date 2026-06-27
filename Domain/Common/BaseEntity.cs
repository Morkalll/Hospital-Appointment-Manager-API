namespace TPI_2026.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();

    private readonly List<BaseEvent> _domainEvents = new List<BaseEvent>();

    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();


    public void AddDomainEvent(BaseEvent baseEvent) => _domainEvents.Add(baseEvent);
    public void RemoveDomainEvent(BaseEvent baseEvent) => _domainEvents.Remove(baseEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public void SoftDelete(DateTime utcNow)
    {
        IsDeleted = true;
        UpdatedAt = utcNow;
        DeletedAt = utcNow;
    }

}