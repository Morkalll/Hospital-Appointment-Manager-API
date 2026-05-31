using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPI_2026.Domain.Enums;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Requests;



namespace TPI_2026.Presentation.Controllers
{

    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {


        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginReq request,
            CancellationToken cancellationToken = default
        )
        {
            var token = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
            return Ok(new { Token = token });
        }

    }
}