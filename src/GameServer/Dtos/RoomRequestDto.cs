using GameServer.Enums;

namespace GameServer.Dtos
{
    public class RoomRequestDto
    {
        public LobbySettingsDto? LobbySettings { get; set; }
        public GameMode GameMode { get; set; }
        public int RoomSize { get; set; }
    }
}
