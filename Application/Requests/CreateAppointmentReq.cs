using System;

namespace TPI_2026.Application.Requests
{
    public record CreateAppointmentReq(
        Guid PatientId,
        Guid DoctorId,
        Guid RoomId,
        DateTime DateTime
    );
}
