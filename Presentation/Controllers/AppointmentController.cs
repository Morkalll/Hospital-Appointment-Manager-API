using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPI_2026.Domain.Enums;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Requests;



namespace TPI_2026.Presentation.Controllers

{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _AppointmentService;

        public AppointmentController(IAppointmentService AppointmentService)
        {
            _AppointmentService = AppointmentService;
        }

        [HttpPost("create-appointment")]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> CreateAppointment(
            [FromBody] CreateAppointmentReq request,
            CancellationToken cancellationToken = default
        )
        {
            var appointmentId = await _AppointmentService.CreateAsync(request.PatientId, request.DoctorId, request.RoomId, request.DateTime, cancellationToken);
            return Ok(new { Id = appointmentId });
        }

        [HttpPut("cancel-appointment/{appointmentId}")]
        [Authorize]
        public async Task<IActionResult> CancelAppointment(
            [FromRoute] Guid appointmentId,
            [FromRoute] bool isDoctor,
            [FromBody] CancelAppointmentReq request,
            CancellationToken cancellationToken = default
        )
        {
            await _AppointmentService.CancelAsync(appointmentId, isDoctor, cancellationToken);
            return Ok();
        }

        [HttpPut("approve-appointment/{appointmentId}")]
        [Authorize(Policy = "Staff")]

        public async Task<IActionResult> ApproveAppointment(
            [FromRoute]
            Guid appointmentId,
            CancellationToken cancellationToken = default
        )
        {
            await _AppointmentService.ApproveAsync(appointmentId, cancellationToken);
            return Ok();
        }

        [HttpGet("patient-appointments/{patientId}")]
        [Authorize]
        public async Task<IActionResult> GetPatientAppointments(
            [FromRoute]
            Guid patientId,
            CancellationToken cancellationToken = default
        )
        {
            var appointments = await _AppointmentService.GetByPatientAsync(patientId, cancellationToken);
            return Ok(appointments);
        }
    }
}
