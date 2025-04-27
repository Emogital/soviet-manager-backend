using GameServer.Services.Gameplay.Matches;

namespace GameServer.Services.Gameplay.Players
{
    public class Player(string userId, string roomName)
    {
        public string UserId => userId;
        public string RoomName => roomName;

        public int Id { get; private set; }
        public PlayerStatus Status { get; private set; }
        public Match? Match { get; private set; }

        public event Action<Player>? StatusChanged;

        public void Initialize(int playerId)
        {
            Id = playerId;
            Status = PlayerStatus.Disconnected;
        }

        public void ChangeStatus(PlayerStatus newStaus)
        {
            Status = newStaus;
            StatusChanged?.Invoke(this);
        }
    }
}
