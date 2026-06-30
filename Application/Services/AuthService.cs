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


public class AuthService(
    IRepository<Doctor> doctorRepo,
    IRepository<Receptionist> receptionistRepo,
    IRepository<Administrator> adminRepo,
    IRepository<Patient> patientRepo,
    IPasswordHasher<Doctor> doctorHasher,
    IPasswordHasher<Receptionist> receptionistHasher,
    IPasswordHasher<Administrator> adminHasher,
    IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        User? user = await doctorRepo.FirstOrDefaultAsync(doctor => doctor.Email == email, cancellationToken)
            ?? (User?)await receptionistRepo.FirstOrDefaultAsync(receptionist => receptionist.Email == email, cancellationToken)
            ?? (User?)await adminRepo.FirstOrDefaultAsync(admin => admin.Email == email, cancellationToken);

        if (user is null)
        {
            var isPatient = await patientRepo.AnyAsync(patient => patient.Email == email, cancellationToken);
            if (isPatient) throw new ForbiddenException("Patients cannot log in to the system.");
            
            throw new NotFoundException("User", email);
        }


        var doctor = await doctorRepo.FirstOrDefaultAsync(d => d.Email == email, cancellationToken);
        if (doctor is not null)
            return AuthenticateAndBuildResponse(doctor, doctor.Password, password, doctorHasher);

        var receptionist = await receptionistRepo.FirstOrDefaultAsync(r => r.Email == email, cancellationToken);
        if (receptionist is not null)
            return AuthenticateAndBuildResponse(receptionist, receptionist.Password, password, receptionistHasher);

        var admin = await adminRepo.FirstOrDefaultAsync(a => a.Email == email, cancellationToken);
        if (admin is not null)
            return AuthenticateAndBuildResponse(admin, admin.Password, password, adminHasher);

        throw new NotFoundException("User", email);
    }

    private AuthResponse AuthenticateAndBuildResponse<T>(T user, string hashedPassword, string plainPassword, IPasswordHasher<T> hasher)
    where T : User
    {
        var result = hasher.VerifyHashedPassword(user, hashedPassword, plainPassword);
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
        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey)) jwtKey = "YourSuperSecretKeyThatIsAtLeast32CharsLong!!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}