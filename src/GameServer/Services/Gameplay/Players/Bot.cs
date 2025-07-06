namespace GameServer.Services.Gameplay.Players
{
    public class Bot(int playerId) : IMatchPlayer
    {
        public string Name { get; } = "Bot" + playerId;
        public int Id { get; } = playerId;
        public int TeamId { get; private set; }
        public PlayerStatus Status { get; } = PlayerStatus.AutoControlled;

        public PlayerData GetData()
        {
            return new PlayerData
            {
                Name = Name,
                Id = Id,
                TeamId = TeamId,
                Status = Status,
            };
        }

        public void SetTeamId(int teamId)
        {
            TeamId = teamId;
        }
    }
}
