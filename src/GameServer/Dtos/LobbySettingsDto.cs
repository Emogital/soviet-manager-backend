using GameServer.Services.Gameplay;

namespace GameServer.Dtos
{
    public class LobbySettingsDto
    {
        public string RoomName { get; init; } = string.Empty;
        public string PlayerName { get; init; } = string.Empty;
        public string PlayerNick { get; init; } = string.Empty;
        public int PlayerRating { get; init; } = 1000;
        public RoomTheme Theme { get; init; }
    }
}
