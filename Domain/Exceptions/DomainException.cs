namespace TPI_2026.Domain.Exceptions;

public class DomainException(string message) : Exception(message) { }

public class NotCancellableAppointmentException(Guid appointmentId)
    : DomainException($"The appointment {appointmentId} cannot be cancelled in its current state.");

public class NotCompleteableAppointmentException(Guid appointmentId)
    : DomainException($"The appointment {appointmentId} cannot be completed in its current state.");
