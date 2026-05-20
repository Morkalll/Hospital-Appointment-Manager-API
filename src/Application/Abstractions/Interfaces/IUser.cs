namespace TPI_2026.Application.Abstractions.Interfaces;

public interface IUser
{
    string? Id { get; }
    List<string>? Roles { get; }

}
