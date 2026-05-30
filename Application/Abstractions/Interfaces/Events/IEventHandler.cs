using TPI_2026.Domain.Common;

namespace TPI_2026.Application.Abstractions.Interfaces.Events;

public interface IEventHandler<TEvent> where TEvent : BaseEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}