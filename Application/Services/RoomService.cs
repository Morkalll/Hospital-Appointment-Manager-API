using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Application.Abstractions.Interfaces.Services;
using TPI_2026.Application.Responses;

namespace TPI_2026.Application.Services;

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoomService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<RoomDto>> GetAllRoomsAsync(CancellationToken cancellationToken = default)
    {
        var rooms = await _unitOfWork.Rooms.GetAllAsync(cancellationToken);
        
        return rooms.Select(r => new RoomDto(
            r.Id,
            r.Number,
            r.Floor,
            r.Specialty,
            r.DoctorId
        )).ToList();
    }
}
