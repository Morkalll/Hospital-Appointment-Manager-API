using TPI_2026.Application.Responses;

namespace TPI_2026.Application.Abstractions.Interfaces.Services;

public interface IRoomService
{
    Task<List<RoomDto>> GetAllRoomsAsync(CancellationToken cancellationToken = default);
}
