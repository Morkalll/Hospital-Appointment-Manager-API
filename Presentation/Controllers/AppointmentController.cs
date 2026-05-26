using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPI_2026.Domain.Enums;
using TPI_2026.Application.Abstractions.Interfaces.Services;



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
        public async Task<IActionResult> CreateAppointment(
            [FromBody]
            Guid patientId,
            Guid doctorId,
            Guid roomId,
            DateTime dateTime,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var appointmentId = await _AppointmentService.CreateAsync(patientId, doctorId, roomId, dateTime, cancellationToken);
                return Ok(new { Id = appointmentId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("cancel-appointment/{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(
            [FromRoute]
            Guid appointmentId,
            [FromBody]
            bool isDoctor,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                await _AppointmentService.CancelAsync(appointmentId, isDoctor, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("approve-appointment/{appointmentId}")]
        public async Task<IActionResult> ApproveAppointment(
            [FromRoute]
            Guid appointmentId,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                await _AppointmentService.ApproveAsync(appointmentId, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("patient-appointments/{patientId}")]
        public async Task<IActionResult> GetPatientAppointments(
            [FromRoute]
            Guid patientId,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var appointments = await _AppointmentService.GetByPatientAsync(patientId, cancellationToken);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
