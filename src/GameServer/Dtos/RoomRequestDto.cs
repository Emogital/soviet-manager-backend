using GameServer.Services.Gameplay;

namespace GameServer.Dtos
{
    public class RoomRequestDto
    {
        public required LobbySettingsDto LobbySettings { get; init; }
        public GameMode GameMode { get; init; }
        public int RoomCapacity { get; init; }
    }
}
