using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Exceptions;
using TPI_2026.Domain.Entities;
using TPI_2026.Application.Responses;

namespace TPI_2026.Application.Services;


public class AuthService(IUnitOfWork unitOfWork, IPasswordHasher<User> hasher, IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        // Busca el usuario en todas las tablas. 
        User? user = await unitOfWork.Patients.FirstOrDefaultAsync(p => p.Email == email, cancellationToken)
            ?? (User?)await unitOfWork.Doctors.FirstOrDefaultAsync(d => d.Email == email, cancellationToken)
            ?? (User?)await unitOfWork.Receptionists.FirstOrDefaultAsync(r => r.Email == email, cancellationToken)
            ?? (User?)await unitOfWork.Administrators.FirstOrDefaultAsync(a => a.Email == email, cancellationToken);

        if (user is null)
            throw new NotFoundException("User", email);

        // compara password con el hash guardado en la base de datos, en caso de ser PasswordVerificationResult.Failed, tira una excepcion
        var result = hasher.VerifyHashedPassword(user, user.Password, password);
        if (result == PasswordVerificationResult.Failed)
            throw new ForbiddenException("Invalid credentials.");

        return new AuthResponse
        {
            Token = GenerateToken(user),
            Role = user.Role.ToString(),
            UserId = user.Id,
            Email = user.Email
        };
    }  

    private string GenerateToken(User user)
    {
        // convierte la clave secreta y la pasa a bytes, porque el hasheo HmacSha256 no lee texto, solo bytes
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Las claims son los datos del usuario que van a viajar dentro del token (el id, el Role, y el email).
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };
        
        // Se genera el token con los datos necesarios.
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );
        
        // Convierte el token a string para devolverlo al cliente (el frontend)
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}