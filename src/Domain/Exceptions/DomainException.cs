namespace Domain.Exceptions;

public class DomainException(string message) : Exception(message) { }

public class NotCancellableAppointmentException(Guid appointmentId)
    : DomainException($"El turno {appointmentId} no puede ser cancelado en su estado actual.");

public class UserNotFoundException(Guid userId)
    : DomainException($"No se encontró el usuario con id {userId}");