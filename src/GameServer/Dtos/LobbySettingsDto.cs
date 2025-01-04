namespace GameServer.Dtos
{
    public class LobbySettingsDto
    {
        public string MatchName { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string PlayerNick { get; set; } = string.Empty;
        public int PlayerRating { get; set; } = 1000;
    }
}
