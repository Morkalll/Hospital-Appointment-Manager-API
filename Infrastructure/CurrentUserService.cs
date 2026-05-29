using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TPI_2026.Application.Abstractions.Interfaces.Services;

namespace TPI_2026.Infrastructure;


// HttpContextAccessor es para acceder al contexto de la request desde cualquier lugar. 
// Se necesita para obtener el Id y el Rol de la claim del token
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    
    public Guid? UserId
    {
        get
        {   
            // Obtiene el valor de la claim NameIdentifier, que seria el Id del usuario y lo parsea a Guid, si falla devuelve null
            var value = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    
       // Obtiene el valor de la claim Role, si no existe devuelve null
    public string? Role => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
}
