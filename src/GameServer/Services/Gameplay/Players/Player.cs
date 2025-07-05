namespace GameServer.Services.Gameplay.Players
{
    public class Player(string userId, string playerName, string roomName, int playerId) : IMatchPlayer
    {
        public string UserId => userId;
        public string Name => playerName;
        public string RoomName => roomName;
        public int Id => playerId;

        public string ConnectionId { get; set; } = string.Empty;
        public int TeamId { get; private set; } = 0;
        public PlayerStatus Status { get; private set; } = PlayerStatus.Disconnected;

        public event Action<Player>? StatusChanged;

        public void ChangeStatus(PlayerStatus newStatus)
        {
            Status = newStatus;
            StatusChanged?.Invoke(this);
        }

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
