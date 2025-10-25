using GameServer.Dtos;

namespace GameServer.Services.Gameplay.Rooms
{
    public interface IRoomService
    {
        bool TryCreateOrJoinRoom(string userId, RoomRequestDto roomRequest);
        bool TryConnectToRoom(string roomName, string userId, string connectionId, out Room? room);
        bool TryGetConnectedRoom(string connectionId, out Room? room);
        Task DisconnectPlayer(string connectionId);
        IReadOnlyCollection<Room> GetAllRooms();
    }
}
