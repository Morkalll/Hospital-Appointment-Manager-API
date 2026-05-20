using MediatR;

namespace TPI_2026.Domain.Common;

// Esta clase abstracta no contiene mucho código porque hereda su comportamiento
// de la interfaz INotification de MediatR, funcionando como un puente
// y como base común para todos los eventos.
public abstract class BaseEvent : INotification
{
}
