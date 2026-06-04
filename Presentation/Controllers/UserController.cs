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
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var users = await _userService.GetAllAsync(cancellationToken);
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> GetById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            return Ok(user);
        }

        [HttpPost("create-patient")]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> CreatePatient(
            [FromBody] CreatePatientReq request,
            CancellationToken cancellationToken = default
        )
        {
            var patientId = await _userService.RegisterPatientAsync(request.Name, request.Email, request.Password, request.Dni, request.BirthDate, request.PhoneNumber, request.Address, cancellationToken);
            return Ok(new { Id = patientId });
        }

        [HttpPost("create-doctor")]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> CreateDoctor(
            [FromBody] CreateDoctorReq request,
            CancellationToken cancellationToken = default
        )
        {
            var doctorId = await _userService.RegisterDoctorAsync(request.Name, request.Email, request.Password, request.Credential, request.Specialty, cancellationToken);
            return Ok(new { Id = doctorId });
        }

        [HttpPost("create-receptionist")]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> CreateReceptionist(
            [FromBody] CreateReceptionistReq request,
            CancellationToken cancellationToken = default
        )
        {
            var receptionistId = await _userService.RegisterReceptionistAsync(request.Name, request.Email, request.Password, request.EmployeeNumber, request.WorkingShift, request.Area, cancellationToken);
            return Ok(new { Id = receptionistId });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> DeleteUser(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _userService.DeleteAsync(id, cancellationToken);
            return Ok(new { Message = "User deleted successfully." });
        }
    }
}
