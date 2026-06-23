using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Exceptions;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;
using TPI_2026.Application.Responses;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;

namespace TPI_2026.Application.Services;

public class UserService(IUnitOfWork unitOfWork, IPasswordHasher<User> hasher) : IUserService
{
    public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var patientsList = await unitOfWork.Patients.GetAllAsync(cancellationToken);
        var patients = patientsList
            .Select(patient => new UserDto(patient.Id, patient.Name, patient.Email, patient.Role.ToString()))
            .ToList();

        var doctorsList = await unitOfWork.Doctors.GetAllAsync(cancellationToken);
        var doctors = doctorsList
            .Select(doctor => new UserDto(doctor.Id, doctor.Name, doctor.Email, doctor.Role.ToString()))
            .ToList();

        var receptionistsList = await unitOfWork.Receptionists.GetAllAsync(cancellationToken);
        var receptionists = receptionistsList
            .Select(receptionist => new UserDto(receptionist.Id, receptionist.Name, receptionist.Email, receptionist.Role.ToString()))
            .ToList();

        var administratorsList = await unitOfWork.Administrators.GetAllAsync(cancellationToken);
        var administrators = administratorsList
            .Select(admin => new UserDto(admin.Id, admin.Name, admin.Email, admin.Role.ToString()))
            .ToList();

        var totalList = new List<UserDto>();
        totalList.AddRange(patients);
        totalList.AddRange(doctors);
        totalList.AddRange(receptionists);
        totalList.AddRange(administrators);

        return totalList;
    }

    public async Task<UserDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.Patients.FirstOrDefaultAsync(patient => patient.Id == userId, cancellationToken);
        if (patient is not null)
            return new UserDto(patient.Id, patient.Name, patient.Email, patient.Role.ToString());

        var doctor = await unitOfWork.Doctors.FirstOrDefaultAsync(doctor => doctor.Id == userId, cancellationToken);
        if (doctor is not null)
            return new UserDto(doctor.Id, doctor.Name, doctor.Email, doctor.Role.ToString());

        var receptionist = await unitOfWork.Receptionists.FirstOrDefaultAsync(receptionist => receptionist.Id == userId, cancellationToken);
        if (receptionist is not null)
            return new UserDto(receptionist.Id, receptionist.Name, receptionist.Email, receptionist.Role.ToString());

        var admin = await unitOfWork.Administrators.FirstOrDefaultAsync(admin => admin.Id == userId, cancellationToken);
        if (admin is not null)
            return new UserDto(admin.Id, admin.Name, admin.Email, admin.Role.ToString());

        throw new NotFoundException("User", userId);
    }

    public async Task<Guid> RegisterPatientAsync(
        string name,
        string email,
        string password,
        string dni,
        DateOnly birthDate,
        string phoneNumber,
        string address,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Name is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new ValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(password)) throw new ValidationException("Password is required.");
        if (string.IsNullOrWhiteSpace(dni)) throw new ValidationException("DNI is required.");

        if (await unitOfWork.Patients.AnyAsync(patient => patient.Dni == dni, cancellationToken))
            throw new ValidationException("A patient with that DNI already exists.");


        if (await unitOfWork.Patients.AnyAsync(p => p.Email == email, cancellationToken) ||
            await unitOfWork.Doctors.AnyAsync(d => d.Email == email, cancellationToken) ||
            await unitOfWork.Receptionists.AnyAsync(r => r.Email == email, cancellationToken) ||
            await unitOfWork.Administrators.AnyAsync(a => a.Email == email, cancellationToken))
        {
            throw new ValidationException("A user with that email already exists.");
        }

        var patient = new Patient
        {
            Name = name,
            Email = email,
            Dni = dni,
            BirthDate = birthDate,
            PhoneNumber = phoneNumber,
            Address = address
        };
        patient.Password = hasher.HashPassword(patient, password);

        unitOfWork.Patients.Add(patient);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return patient.Id;
    }

    public async Task<Guid> RegisterDoctorAsync(
        string name,
        string email,
        string password,
        string credential,
        Specialty specialty,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Name is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new ValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(password)) throw new ValidationException("Password is required.");
        if (string.IsNullOrWhiteSpace(credential)) throw new ValidationException("Credential is required.");

        if (await unitOfWork.Patients.AnyAsync(p => p.Email == email, cancellationToken) ||
            await unitOfWork.Doctors.AnyAsync(d => d.Email == email, cancellationToken) ||
            await unitOfWork.Receptionists.AnyAsync(r => r.Email == email, cancellationToken) ||
            await unitOfWork.Administrators.AnyAsync(a => a.Email == email, cancellationToken))
        {
            throw new ValidationException("A user with that email already exists.");
        }

        if (await unitOfWork.Doctors.AnyAsync(doctor => doctor.Credential == credential, cancellationToken))
            throw new ValidationException("A doctor with that credential already exists.");

        var doctor = new Doctor
        {
            Name = name,
            Email = email,
            Credential = credential,
            Specialty = specialty
        };
        doctor.Password = hasher.HashPassword(doctor, password);

        unitOfWork.Doctors.Add(doctor);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return doctor.Id;
    }

    public async Task<Guid> RegisterReceptionistAsync(
        string name,
        string email,
        string password,
        string employeeNumber,
        string workingShift,
        string area,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Name is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new ValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(password)) throw new ValidationException("Password is required.");
        if (string.IsNullOrWhiteSpace(employeeNumber)) throw new ValidationException("Employee number is required.");
        if (string.IsNullOrWhiteSpace(workingShift)) throw new ValidationException("Working shift is required.");
        if (string.IsNullOrWhiteSpace(area)) throw new ValidationException("Area is required.");

        if (await unitOfWork.Patients.AnyAsync(p => p.Email == email, cancellationToken) ||
            await unitOfWork.Doctors.AnyAsync(d => d.Email == email, cancellationToken) ||
            await unitOfWork.Receptionists.AnyAsync(r => r.Email == email, cancellationToken) ||
            await unitOfWork.Administrators.AnyAsync(a => a.Email == email, cancellationToken))
        {
            throw new ValidationException("A user with that email already exists.");
        }

        if (await unitOfWork.Receptionists.AnyAsync(receptionist => receptionist.EmployeeNumber == employeeNumber, cancellationToken))
            throw new ValidationException("A user with that employee number already exists.");

        var receptionist = new Receptionist
        {
            Name = name,
            Email = email,
            EmployeeNumber = employeeNumber,
            WorkingShift = workingShift,
            Area = area
        };

        receptionist.Password = hasher.HashPassword(receptionist, password);

        unitOfWork.Receptionists.Add(receptionist);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return receptionist.Id;
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var patient = await unitOfWork.Patients.GetByIdAsync(userId, cancellationToken);
        if (patient is not null)
        {
            patient.IsDeleted = true;
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var doctor = await unitOfWork.Doctors.GetByIdAsync(userId, cancellationToken);
        if (doctor is not null)
        {
            doctor.IsDeleted = true;
            doctor.IsAvailable = false;
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var receptionist = await unitOfWork.Receptionists.GetByIdAsync(userId, cancellationToken);
        if (receptionist is not null)
        {
            receptionist.IsDeleted = true;
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }


        throw new NotFoundException("User", userId);
    }
}