using GameServer.Dtos;

namespace GameServer.Services.Gameplay.Rooms
{
    public interface IRoomService
    {
        RoomData? GetRoomData(string roomName);
        bool TryCreateOrJoinRoom(string userId, RoomRequestDto roomRequest);
    }
}
