using System.Globalization;
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
        bool isDoctor,
        CancellationToken cancellationToken = default);

    Task ApproveAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default);

    Task<List<AppointmentDto>> GetByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}


