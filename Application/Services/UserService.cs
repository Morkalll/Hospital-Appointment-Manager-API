using Microsoft.EntityFrameworkCore;
using TPI_2026.Application.Abstractions.Interfaces;
using TPI_2026.Application.Exceptions;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;

namespace TPI_2026.Application.Services;

public class UserService(IApplicationDbContext dataBase) : IUserService
{
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


        if (await dataBase.Patients.AnyAsync(patient => patient.Dni == dni, cancellationToken))
            throw new ForbiddenException("A patient with that DNI already exists.");

        // Valida si el email ya existe en alguna de las tres tablas de usuarios.
        if (await dataBase.Receptionists.AnyAsync(receptionist => receptionist.Email == email, cancellationToken) ||
            await dataBase.Patients.AnyAsync(patient => patient.Email == email, cancellationToken) ||
            await dataBase.Doctors.AnyAsync(doctor => doctor.Email == email, cancellationToken))
        {
            throw new ForbiddenException("A user with that email already exists.");
        }

        var patient = new Patient
        {
            Name = name,
            Email = email,
            Password = password,
            Dni = dni,
            BirthDate = birthDate,
            PhoneNumber = phoneNumber,
            Adress = adress
        };

        dataBase.Patients.Add(patient);
        await dataBase.SaveChangesAsync(cancellationToken);
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
        if (await dataBase.Receptionists.AnyAsync(receptionist => receptionist.Email == email, cancellationToken) ||
            await dataBase.Patients.AnyAsync(patient => patient.Email == email, cancellationToken) ||
            await dataBase.Doctors.AnyAsync(doctor => doctor.Email == email, cancellationToken))
        {
            throw new ForbiddenException("A user with that email already exists.");
        }

        if (await dataBase.Doctors.AnyAsync(doctor => doctor.Credential == credential, cancellationToken))
            throw new ForbiddenException("A doctor with that credential already exists.");

        var doctor = new Doctor
        {
            Name = name,
            Email = email,
            Password = password,
            Credential = credential,
            Specialty = specialty
        };

        dataBase.Doctors.Add(doctor);
        await dataBase.SaveChangesAsync(cancellationToken);
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
        if (await dataBase.Receptionists.AnyAsync(receptionist => receptionist.Email == email, cancellationToken) ||
            await dataBase.Patients.AnyAsync(patient => patient.Email == email, cancellationToken) ||
            await dataBase.Doctors.AnyAsync(doctor => doctor.Email == email, cancellationToken))
        {
            throw new ForbiddenException("A user with that email already exists.");
        }

        if (await dataBase.Receptionists.AnyAsync(receptionist => receptionist.EmployeeNumber == employeeNumber, cancellationToken))
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

        dataBase.Receptionists.Add(receptionist);
        await dataBase.SaveChangesAsync(cancellationToken);
        return receptionist.Id;
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var patient = await dataBase.Patients.FirstOrDefaultAsync(patient => patient.Id == userId, cancellationToken);
        if (patient is not null)
        {
            dataBase.Patients.Remove(patient); await dataBase.SaveChangesAsync(cancellationToken);
            return;
        }

        var doctor = await dataBase.Doctors.FirstOrDefaultAsync(doctor => doctor.Id == userId, cancellationToken);
        if (doctor is not null)
        {
            dataBase.Doctors.Remove(doctor); await dataBase.SaveChangesAsync(cancellationToken);
            return;
        }

        var receptionist = await dataBase.Receptionists.FirstOrDefaultAsync(receptionist => receptionist.Id == userId, cancellationToken);
        if (receptionist is not null)
        {
            dataBase.Receptionists.Remove(receptionist); await dataBase.SaveChangesAsync(cancellationToken);
            return;
        }

        throw new NotFoundException("User", userId);
    }
}
