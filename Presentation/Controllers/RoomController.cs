using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPI_2026.Application.Abstractions.Interfaces.Services;

namespace TPI_2026.Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRooms(CancellationToken cancellationToken = default)
    {
        var rooms = await _roomService.GetAllRoomsAsync(cancellationToken);
        return Ok(rooms);
    }
}
