using GameServer.Enums;

namespace GameServer.Dtos
{
    public class RoomRequestDto
    {
        public required LobbySettingsDto LobbySettings { get; set; }
        public GameMode GameMode { get; set; }
        public int RoomSize { get; set; }
    }
}
