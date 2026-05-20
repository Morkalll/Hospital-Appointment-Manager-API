using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TPI_2026.Infrastructure.ExternalServices;

namespace TPI_2026.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UsersController(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] object empty)
    {
        if (empty != null)
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        return Unauthorized();
    }
}
