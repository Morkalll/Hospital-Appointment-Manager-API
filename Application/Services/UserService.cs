using Microsoft.AspNetCore.Identity;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Exceptions;
using TPI_2026.Domain.Entities;
using TPI_2026.Domain.Enums;
using TPI_2026.Application.Responses;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using System.Text.RegularExpressions;

namespace TPI_2026.Application.Services;

public class UserService(
    IRepository<Patient> patientRepo,
    IRepository<Doctor> doctorRepo,
    IRepository<Receptionist> receptionistRepo,
    IRepository<Administrator> adminRepo,
    IPasswordHasher<Doctor> doctorHasher,
    IPasswordHasher<Receptionist> receptionistHasher,
    IPasswordHasher<Administrator> adminHasher) : IUserService
{
    public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var patientsList = await patientRepo.GetAllAsync(cancellationToken);
        var patients = patientsList
            .Select(patient => new UserDto(patient.Id, patient.Name, patient.Email, patient.Role.ToString()))
            .ToList();

        var doctorsList = await doctorRepo.GetAllAsync(cancellationToken);
        var doctors = doctorsList
            .Select(doctor => new UserDto(doctor.Id, doctor.Name, doctor.Email, doctor.Role.ToString()))
            .ToList();

        var receptionistsList = await receptionistRepo.GetAllAsync(cancellationToken);
        var receptionists = receptionistsList
            .Select(receptionist => new UserDto(receptionist.Id, receptionist.Name, receptionist.Email, receptionist.Role.ToString()))
            .ToList();

        var administratorsList = await adminRepo.GetAllAsync(cancellationToken);
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
        var patient = await patientRepo.FirstOrDefaultAsync(patient => patient.Id == userId, cancellationToken);
        if (patient is not null)
            return new PatientDto(patient.Id, patient.Name, patient.Email, patient.Role.ToString(), patient.Dni, patient.BirthDate, patient.PhoneNumber, patient.Address);

        var doctor = await doctorRepo.FirstOrDefaultAsync(doctor => doctor.Id == userId, cancellationToken);
        if (doctor is not null)
            return new DoctorDto(doctor.Id, doctor.Name, doctor.Email, doctor.Role.ToString(), doctor.Credential, doctor.Specialty, doctor.IsAvailable);

        var receptionist = await receptionistRepo.FirstOrDefaultAsync(receptionist => receptionist.Id == userId, cancellationToken);
        if (receptionist is not null)
            return new ReceptionistDto(receptionist.Id, receptionist.Name, receptionist.Email, receptionist.Role.ToString(), receptionist.EmployeeNumber, receptionist.WorkingShift, receptionist.Area);

        var admin = await adminRepo.FirstOrDefaultAsync(admin => admin.Id == userId, cancellationToken);
        if (admin is not null)
            return new UserDto(admin.Id, admin.Name, admin.Email, admin.Role.ToString());

        throw new NotFoundException("User");
    }

    public async Task<Guid> RegisterPatientAsync(
        string name,
        string email,
        string dni,
        DateOnly birthDate,
        string phoneNumber,
        string address,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name)) errors.Add("Name is required.");
        
        if (string.IsNullOrWhiteSpace(email)) errors.Add("Email is required.");
        else if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) errors.Add("Invalid email format.");

        if (string.IsNullOrWhiteSpace(dni)) errors.Add("DNI is required.");
        else if (!Regex.IsMatch(dni, @"^\d{7,8}$")) errors.Add("DNI must be 7 or 8 digits.");

        if (birthDate > DateOnly.FromDateTime(DateTime.UtcNow)) errors.Add("Birth date cannot be in the future.");

        if (string.IsNullOrWhiteSpace(phoneNumber)) errors.Add("Phone number is required.");
        else if (!Regex.IsMatch(phoneNumber, @"^\d+$")) errors.Add("Phone number must contain only numbers.");  
        
        if (string.IsNullOrWhiteSpace(address)) errors.Add("Address is required.");

        if (errors.Count > 0) throw new ValidationException(errors);

        if (await patientRepo.AnyAsync(patient => patient.Dni == dni, cancellationToken))
            errors.Add("A patient with that DNI already exists.");

        if (await patientRepo.AnyAsync(patient => patient.Email == email, cancellationToken) ||
            await doctorRepo.AnyAsync(doctor => doctor.Email == email, cancellationToken) ||
            await receptionistRepo.AnyAsync(receptionist => receptionist.Email == email, cancellationToken) ||
            await adminRepo.AnyAsync(admin => admin.Email == email, cancellationToken))
        {
            errors.Add("A user with that email already exists.");
        }

        if (errors.Count > 0) throw new ValidationException(errors);

        var patient = new Patient
        {
            Name = name,
            Email = email,
            Dni = dni,
            BirthDate = birthDate,
            PhoneNumber = phoneNumber,
            Address = address
        };

        await patientRepo.AddAsync(patient, cancellationToken);
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
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name)) errors.Add("Name is required.");
        
        if (string.IsNullOrWhiteSpace(email)) errors.Add("Email is required.");
        else if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) errors.Add("Invalid email format.");

        if (string.IsNullOrWhiteSpace(password)) errors.Add("Password is required.");
        else if (password.Length < 6) errors.Add("Password must be at least 6 characters long.");
        
        if (string.IsNullOrWhiteSpace(credential)) errors.Add("Credential is required.");

        if (errors.Count > 0) throw new ValidationException(errors);

        if (await patientRepo.AnyAsync(patient => patient.Email == email, cancellationToken) ||
            await doctorRepo.AnyAsync(doctor => doctor.Email == email, cancellationToken) ||
            await receptionistRepo.AnyAsync(receptionist => receptionist.Email == email, cancellationToken) ||
            await adminRepo.AnyAsync(admin => admin.Email == email, cancellationToken))
        {
            errors.Add("A user with that email already exists.");
        }

        if (await doctorRepo.AnyAsync(doctor => doctor.Credential == credential, cancellationToken))
            errors.Add("A doctor with that credential already exists.");

        if (errors.Count > 0) throw new ValidationException(errors);



        var doctor = new Doctor
        {
            Name = name,
            Email = email,
            Credential = credential,
            Specialty = specialty
        };
        doctor.Password = doctorHasher.HashPassword(doctor, password);

        await doctorRepo.AddAsync(doctor, cancellationToken);
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
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name)) errors.Add("Name is required.");

        if (string.IsNullOrWhiteSpace(email)) errors.Add("Email is required.");
        else if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) errors.Add("Invalid email format.");

        if (string.IsNullOrWhiteSpace(password)) errors.Add("Password is required.");
        else if (password.Length < 6) errors.Add("Password must be at least 6 characters long.");

        if (string.IsNullOrWhiteSpace(employeeNumber)) errors.Add("Employee number is required.");

        if (string.IsNullOrWhiteSpace(workingShift)) errors.Add("Working shift is required.");

        if (string.IsNullOrWhiteSpace(area)) errors.Add("Area is required.");

        if (errors.Count > 0) throw new ValidationException(errors);

        if (await patientRepo.AnyAsync(patient => patient.Email == email, cancellationToken) ||
            await doctorRepo.AnyAsync(doctor => doctor.Email == email, cancellationToken) ||
            await receptionistRepo.AnyAsync(receptionist => receptionist.Email == email, cancellationToken) ||
            await adminRepo.AnyAsync(admin => admin.Email == email, cancellationToken))
        {
            errors.Add("A user with that email already exists.");
        }

        if (await receptionistRepo.AnyAsync(receptionist => receptionist.EmployeeNumber == employeeNumber, cancellationToken))
            errors.Add("A user with that employee number already exists.");

        if (errors.Count > 0) throw new ValidationException(errors);

        var receptionist = new Receptionist
        {
            Name = name,
            Email = email,
            EmployeeNumber = employeeNumber,
            WorkingShift = workingShift,
            Area = area
        };

        receptionist.Password = receptionistHasher.HashPassword(receptionist, password);

        await receptionistRepo.AddAsync(receptionist, cancellationToken);
        return receptionist.Id;
    }

    public async Task<Guid> RegisterAdminAsync(
        string name,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name)) errors.Add("Name is required.");

        if (string.IsNullOrWhiteSpace(email)) errors.Add("Email is required.");
        else if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) errors.Add("Invalid email format.");

        if (string.IsNullOrWhiteSpace(password)) errors.Add("Password is required.");
        else if (password.Length < 6) errors.Add("Password must be at least 6 characters long.");

        if (errors.Count > 0) throw new ValidationException(errors);
        
        if (await patientRepo.AnyAsync(patient => patient.Email == email, cancellationToken) ||
            await doctorRepo.AnyAsync(doctor => doctor.Email == email, cancellationToken) ||
            await receptionistRepo.AnyAsync(receptionist => receptionist.Email == email, cancellationToken) ||
            await adminRepo.AnyAsync(admin => admin.Email == email, cancellationToken))
        {
            errors.Add("A user with that email already exists.");
        }

        if (errors.Count > 0) throw new ValidationException(errors);


        var administrator = new Administrator
        {
            Name = name,
            Email = email,
        };

        administrator.Password = adminHasher.HashPassword(administrator, password);

        await adminRepo.AddAsync(administrator, cancellationToken);
        return administrator.Id;
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var patient = await patientRepo.GetByIdAsync(userId, cancellationToken);
        if (patient is not null)
        {
            patient.IsDeleted = true;
            await patientRepo.UpdateAsync(patient, cancellationToken);
            return;
        }

        var doctor = await doctorRepo.GetByIdAsync(userId, cancellationToken);
        if (doctor is not null)
        {
            doctor.IsDeleted = true;
            doctor.IsAvailable = false;
            await doctorRepo.UpdateAsync(doctor, cancellationToken);
            return;
        }

        var receptionist = await receptionistRepo.GetByIdAsync(userId, cancellationToken);
        if (receptionist is not null)
        {
            receptionist.IsDeleted = true;
            await receptionistRepo.UpdateAsync(receptionist, cancellationToken);
            return;
        }

        var admin = await adminRepo.GetByIdAsync(userId, cancellationToken);
        if (admin is not null)
        {
            var allAdmins = await adminRepo.GetAllAsync(cancellationToken);
            if (allAdmins.Count <= 1)
            {
                var errors = new List<string> { "Cannot delete the last remaining administrator." };
                throw new ValidationException(errors);
            }

            admin.IsDeleted = true;
            await adminRepo.UpdateAsync(admin, cancellationToken);
            return;
        }

        throw new NotFoundException("User");
    }
}