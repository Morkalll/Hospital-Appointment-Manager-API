using TPI_2026.Application.Responses;

namespace TPI_2026.Application.Abstractions.Interfaces.Services;

public interface IAppointmentService
{
    Task<Guid> CreateAsync(
        Guid patientId,
        Guid doctorId,
        Guid roomId,
        DateTime dateTime,
        CancellationToken
        cancellationToken = default);

    Task CancelAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default);

    Task CompletionAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default
    );

    Task<List<AppointmentDto>> GetByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<List<AppointmentDto>> GetByDoctorAsync(
        Guid doctorId,
        CancellationToken cancellationToken = default);
}


