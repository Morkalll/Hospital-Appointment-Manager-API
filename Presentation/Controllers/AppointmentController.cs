using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Requests;



namespace TPI_2026.Presentation.Controllers

{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService AppointmentService)
        {
            _appointmentService = AppointmentService;
        }

        [HttpPost("create-appointment")]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> CreateAppointment(
            [FromBody] CreateAppointmentReq request,
            CancellationToken cancellationToken = default
        )
        {
            var appointmentId = await _appointmentService.CreateAsync(request.PatientId, request.DoctorId, request.RoomId, request.DateTime, cancellationToken);
            return Ok(new { Id = appointmentId });
        }

        [HttpPut("cancel-appointment/{appointmentId}")]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> CancelAppointment(
            [FromRoute] Guid appointmentId,
            [FromBody] CancelAppointmentReq request,
            CancellationToken cancellationToken = default
        )
        {
            await _appointmentService.CancelAsync(appointmentId, cancellationToken);
            return Ok();
        }

        [HttpPut("complete-appointment/{appointmentId}")]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> CompleteAppointment(
            [FromRoute] Guid appointmentId,
            [FromBody] CompleteAppointmentReq request,
            CancellationToken cancellationToken = default
        )
        {
            await _appointmentService.CompletionAsync(appointmentId, cancellationToken);
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
            var appointments = await _appointmentService.GetByPatientAsync(patientId, cancellationToken);
            return Ok(appointments);
        }
    }
}
