using GameServer.Dtos;

namespace GameServer.Services.Gameplay.Rooms
{
    public interface IRoomService
    {
        bool TryCreateOrJoinRoom(string userId, RoomRequestDto roomRequest);
    }
}
