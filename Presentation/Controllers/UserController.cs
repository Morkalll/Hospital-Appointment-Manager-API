using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPI_2026.Domain.Enums;
using TPI_2026.Application.Abstractions.Interfaces.Services;



namespace TPI_2026.Presentation.Controllers

{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _UserService;

        public UserController(IUserService userService)
        {
            _UserService = userService;
        }

        [HttpPost("create-patient")]
        public async Task<IActionResult> CreatePatient(
            [FromBody]
            string name,
            string email,
            string password,
            string dni,
            string birthDate,
            string phoneNumber,
            string adress,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var patientId = await _UserService.RegisterPatientAsync(name, email, password, dni, birthDate, phoneNumber, adress, cancellationToken);
                return Ok(new { Id = patientId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("create-doctor")]
        public async Task<IActionResult> CreateDoctor(
            [FromBody]
            string name,
            string email,
            string password,
            string credential,
            Specialty specialty,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var doctorId = await _UserService.RegisterDoctorAsync(name, email, password, credential, specialty, cancellationToken);
                return Ok(new { Id = doctorId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [HttpPost("create-receptionist")]
        public async Task<IActionResult> CreateReceptionist(
            [FromBody]
            string name,
            string email,
            string password,
            string employeeNumber,
            string workingShift,
            string area,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var receptionistId = await _UserService.RegisterReceptionistAsync(name, email, password, employeeNumber, workingShift, area, cancellationToken);
                return Ok(new { Id = receptionistId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _UserService.DeleteAsync(id, cancellationToken);
                return Ok(new { Message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
