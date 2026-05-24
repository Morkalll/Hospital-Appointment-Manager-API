using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TPI_2026.Application.Abstractions.Interfaces;
using TPI_2026.Application.Exceptions;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;
using TPI_2026.Application.Responses;

namespace TPI_2026.Application.Services;

public class UserService(IApplicationDbContext database, IPasswordHasher<User> hasher) : IUserService
{
    public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var patients = await database.Patients
            .Select(patient => new UserDto(patient.Id, patient.Name, patient.Email, patient.Role.ToString()))
            .ToListAsync(cancellationToken);

        var doctors = await database.Doctors
            .Select(doctor => new UserDto(doctor.Id, doctor.Name, doctor.Email, doctor.Role.ToString()))
            .ToListAsync(cancellationToken);

        var receptionists = await database.Receptionists
            .Select(receptionist => new UserDto(receptionist.Id, receptionist.Name, receptionist.Email, receptionist.Role.ToString()))
            .ToListAsync(cancellationToken);

        var administrators = await database.Administrators
            .Select(admin => new UserDto(admin.Id, admin.Name, admin.Email, admin.Role.ToString()))
            .ToListAsync(cancellationToken);

        return [.. patients, .. doctors, .. receptionists, .. administrators];
    }

    public async Task<UserDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var patient = await database.Patients.FirstOrDefaultAsync(patient => patient.Id == userId, cancellationToken);
        if (patient is not null)
            return new UserDto(patient.Id, patient.Name, patient.Email, patient.Role.ToString());

        var doctor = await database.Doctors.FirstOrDefaultAsync(doctor => doctor.Id == userId, cancellationToken);
        if (doctor is not null)
            return new UserDto(doctor.Id, doctor.Name, doctor.Email, doctor.Role.ToString());

        var receptionist = await database.Receptionists.FirstOrDefaultAsync(receptionist => receptionist.Id == userId, cancellationToken);
        if (receptionist is not null)
            return new UserDto(receptionist.Id, receptionist.Name, receptionist.Email, receptionist.Role.ToString());

        var admin = await database.Administrators.FirstOrDefaultAsync(admin => admin.Id == userId, cancellationToken);
        if (admin is not null)
            return new UserDto(admin.Id, admin.Name, admin.Email, admin.Role.ToString());

        throw new NotFoundException("User", userId);
    }

    public async Task<Guid> CreatePatientAsync(
        string name,
        string email,
        string password,
        string dni,
        string birthDate,
        string phoneNumber,
        string adress,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ForbiddenException("Name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ForbiddenException("Email is required.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ForbiddenException("Password is required.");

        if (string.IsNullOrWhiteSpace(dni))
            throw new ForbiddenException("DNI is required.");


        if (await database.Patients.AnyAsync(patient => patient.Dni == dni, cancellationToken))
            throw new ForbiddenException("A patient with that DNI already exists.");

        // Valida si el email ya existe en alguna de las tres tablas de usuarios.
        if (await database.Receptionists.AnyAsync(receptionist => receptionist.Email == email, cancellationToken) ||
            await database.Patients.AnyAsync(patient => patient.Email == email, cancellationToken) ||
            await database.Doctors.AnyAsync(doctor => doctor.Email == email, cancellationToken))
        {
            throw new ForbiddenException("A user with that email already exists.");
        }

        var patient = new Patient
        {
            Name = name,
            Email = email,
            Dni = dni,
            BirthDate = birthDate,
            PhoneNumber = phoneNumber,
            Adress = adress
        };
        patient.Password = hasher.HashPassword(patient, password);

        database.Patients.Add(patient);
        await database.SaveChangesAsync(cancellationToken);
        return patient.Id;
    }

    public async Task<Guid> CreateDoctorAsync(
        string name,
        string email,
        string password,
        string credential,
        Specialty specialty,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ForbiddenException("Name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ForbiddenException("Email is required.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ForbiddenException("Password is required.");

        if (string.IsNullOrWhiteSpace(credential))
            throw new ForbiddenException("Credential is required.");


        // Valida si el email ya existe en alguna de las tres tablas de usuarios.
        if (await database.Receptionists.AnyAsync(receptionist => receptionist.Email == email, cancellationToken) ||
            await database.Patients.AnyAsync(patient => patient.Email == email, cancellationToken) ||
            await database.Doctors.AnyAsync(doctor => doctor.Email == email, cancellationToken))
        {
            throw new ForbiddenException("A user with that email already exists.");
        }

        if (await database.Doctors.AnyAsync(doctor => doctor.Credential == credential, cancellationToken))
            throw new ForbiddenException("A doctor with that credential already exists.");

        var doctor = new Doctor
        {
            Name = name,
            Email = email,
            Credential = credential,
            Specialty = specialty
        };
        doctor.Password = hasher.HashPassword(doctor, password);

        database.Doctors.Add(doctor);
        await database.SaveChangesAsync(cancellationToken);
        return doctor.Id;
    }

    public async Task<Guid> CreateReceptionistAsync(
        string name,
        string email,
        string password,
        string employeeNumber,
        string workingShift,
        string area,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ForbiddenException("Name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ForbiddenException("Email is required.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ForbiddenException("Password is required.");

        if (string.IsNullOrWhiteSpace(employeeNumber))
            throw new ForbiddenException("Employee number is required.");

        if (string.IsNullOrWhiteSpace(workingShift))
            throw new ForbiddenException("Working shift is required.");

        if (string.IsNullOrWhiteSpace(area))
            throw new ForbiddenException("Area is required.");


        // Valida si el email ya existe en alguna de las tres tablas de usuarios.
        if (await database.Receptionists.AnyAsync(receptionist => receptionist.Email == email, cancellationToken) ||
            await database.Patients.AnyAsync(patient => patient.Email == email, cancellationToken) ||
            await database.Doctors.AnyAsync(doctor => doctor.Email == email, cancellationToken))
        {
            throw new ForbiddenException("A user with that email already exists.");
        }

        if (await database.Receptionists.AnyAsync(receptionist => receptionist.EmployeeNumber == employeeNumber, cancellationToken))
            throw new ForbiddenException("A user with that employee number already exists.");

        var receptionist = new Receptionist
        {
            Name = name,
            Email = email,
            Password = password,
            EmployeeNumber = employeeNumber,
            WorkingShift = workingShift,
            Area = area
        };

        database.Receptionists.Add(receptionist);
        await database.SaveChangesAsync(cancellationToken);
        return receptionist.Id;
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var patient = await database.Patients.FirstOrDefaultAsync(patient => patient.Id == userId, cancellationToken);
        if (patient is not null)
        {
            database.Patients.Remove(patient); await database.SaveChangesAsync(cancellationToken);
            return;
        }

        var doctor = await database.Doctors.FirstOrDefaultAsync(doctor => doctor.Id == userId, cancellationToken);
        if (doctor is not null)
        {
            database.Doctors.Remove(doctor); await database.SaveChangesAsync(cancellationToken);
            return;
        }

        var receptionist = await database.Receptionists.FirstOrDefaultAsync(receptionist => receptionist.Id == userId, cancellationToken);
        if (receptionist is not null)
        {
            database.Receptionists.Remove(receptionist); await database.SaveChangesAsync(cancellationToken);
            return;
        }

        throw new NotFoundException("User", userId);
    }
}
