using GameServer.Dtos;
using GameServer.Services.Gameplay.Players;

namespace GameServer.Services.Gameplay.Rooms
{
    public class Room : IDisposable
    {
        public string Name { get; }
        public Player?[] Players { get; }

        public RoomStatus Status { get; private set; }

        public event Action<Room>? StatusChanged;

        public Room(RoomRequestDto roomRequest)
        {
            Name = roomRequest.LobbySettings.RoomName;
            Players = new Player[roomRequest.RoomSize];

            Status = RoomStatus.Awaiting;
        }

        public bool IsAvailableToJoin()
        {
            foreach (var player in Players)
            {
                if (player == null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsRoomEmpty()
        {
            foreach (var player in Players)
            {
                if (player != null && player.Status == PlayerStatus.Connected)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsSelfRoomOrEmpty(string userId)
        {
            foreach (var player in Players)
            {
                if (player == null)
                {
                    continue;
                }
            
                if (player.UserId != userId && player.Status == PlayerStatus.Connected)
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryRegisterPlayer(Player player)
        {
            for (int playerId = 0; playerId < Players.Length; playerId++)
            {
                if (Players[playerId] == null)
                {
                    Players[playerId] = player;
                    player.Initialize(playerId);
                    player.StatusChanged += OnPlayerStatusChanged;
                    return true;
                }
            }

            return false;
        }
        
        public bool TryRemovePlayer(int playerId, out Player? player)
        {
            player = Players[playerId];
            if (player != null)
            {
                player.StatusChanged -= OnPlayerStatusChanged;
                Players[playerId] = null;
                return true;
            }

            return false;
        }

        private void OnPlayerStatusChanged(Player player)
        {
            if (player.Status == PlayerStatus.Removed)
            {
                player.StatusChanged -= OnPlayerStatusChanged;
            }

            if(player.Status == PlayerStatus.Connected)
            {
                return;
            }

            if (IsRoomEmpty())
            {
                Status = RoomStatus.Removing;
                StatusChanged?.Invoke(this);
            }
        }

        public void Dispose()
        {
            for (int playerId = 0; playerId < Players.Length; playerId++)
            {
                if (Players[playerId] is Player player && player.Status != PlayerStatus.Removed)
                {
                    player.StatusChanged -= OnPlayerStatusChanged;
                    player.ChangeStatus(PlayerStatus.Removed);
                }

                Players[playerId] = null;
            }
        }
    }
}
