namespace TPI_2026.Application.Exceptions;

public class NotFoundException(string name, object key)
    : Exception($"'{name}' ({key}) was not found.");

// Heredan de la clase Exception, la cual retorna el mensaje creado.

public class ForbiddenException(string message = "Access denied.")
    : Exception(message);

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }
    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors ocurred.")
    {
        Errors = errors;
    }
}