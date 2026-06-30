using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Responses;

using TPI_2026.Domain.Entities;

namespace TPI_2026.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRepository<Room> _roomRepository;

    public RoomService(IRepository<Room> roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<List<RoomDto>> GetAllRoomsAsync(CancellationToken cancellationToken = default)
    {
        var rooms = await _roomRepository.GetAllAsync(cancellationToken);
        
        return rooms.Select(r => new RoomDto(
            r.Id,
            r.Number,
            r.Floor,
            r.Specialty,
            r.DoctorId
        )).ToList();
    }
}
