using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TPI_2026.Application.Abstractions.Interfaces.Services;

namespace TPI_2026.Infrastructure;


public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    
    public Guid? UserId
    {
        get
        {   
            var value = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    
    public string? Role => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
}
