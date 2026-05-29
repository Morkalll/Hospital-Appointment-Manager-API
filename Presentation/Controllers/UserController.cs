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
    public class UserController : ControllerBase
    {
        private readonly IUserService _UserService;

        public UserController(IUserService userService)
        {
            _UserService = userService;
        }

        [HttpPost("create-patient")]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> CreatePatient(
            [FromBody] CreatePatientReq request,
            CancellationToken cancellationToken = default
        )
        {
            var patientId = await _UserService.RegisterPatientAsync(request.Name, request.Email, request.Password, request.Dni, request.BirthDate, request.PhoneNumber, request.Adress, cancellationToken);
            return Ok(new { Id = patientId });
        }

        [HttpPost("create-doctor")]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> CreateDoctor(
            [FromBody] CreateDoctorReq request,
            CancellationToken cancellationToken = default
        )
        {
            var doctorId = await _UserService.RegisterDoctorAsync(request.Name, request.Email, request.Password, request.Credential, request.Specialty, cancellationToken);
            return Ok(new { Id = doctorId });
        }

        [HttpPost("create-receptionist")]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> CreateReceptionist(
            [FromBody] CreateReceptionistReq request,
            CancellationToken cancellationToken = default
        )
        {
            var receptionistId = await _UserService.RegisterReceptionistAsync(request.Name, request.Email, request.Password, request.EmployeeNumber, request.WorkingShift, request.Area, cancellationToken);
            return Ok(new { Id = receptionistId });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> DeleteUser(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _UserService.DeleteAsync(id, cancellationToken);
            return Ok(new { Message = "User deleted successfully." });
        }
    }
}
