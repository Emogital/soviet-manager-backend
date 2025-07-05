namespace GameServer.Services.Gameplay.Players
{
    public class Bot : IMatchPlayer
    {
        public int Id { get; }
        public int TeamId { get; private set; }

        public Bot(int playerId, GameMode gameMode)
        {
            Id = playerId;
        }

        public PlayerData GetData()
        {
            return new PlayerData
            {
                Name = "Bot",
                Id = Id,
                TeamId = TeamId,
                Status = PlayerStatus.Connected,
            };
        }

        public void SetTeamId(int teamId)
        {
            TeamId = teamId;
        }
    }
}
