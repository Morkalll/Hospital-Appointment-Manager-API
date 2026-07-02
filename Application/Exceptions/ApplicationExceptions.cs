namespace TPI_2026.Application.Exceptions;

public class NotFoundException(string name)
    : Exception($"'{name}' was not found.");

public class ForbiddenException(string message = "Access denied.")
    : Exception(message);

public class ValidationException : Exception
{
    public IEnumerable<string> Errors { get; }
    
    public ValidationException(IEnumerable<string> errors)
        : base("One or more validation errors ocurred.")
    {
        Errors = errors;
    }
}