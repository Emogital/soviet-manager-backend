using GameServer.Dtos;
using GameServer.Enums;
using GameServer.Hubs;
using GameServer.Services.Gameplay.Matches;
using GameServer.Services.Gameplay.Players;
using Microsoft.AspNetCore.SignalR;

namespace GameServer.Services.Gameplay.Rooms
{
    public class Room : IDisposable
    {
        private readonly IHubContext<MatchHub> hubContext;

        public readonly string Name;
        public readonly GameMode GameMode;
        public readonly Player?[] Players;

        public RoomStatus Status { get; private set; }
        public Match? Match { get; private set; }

        public RoomData Data => new RoomData(this);

        public event Action<Room>? StatusChanged;

        public Room(IHubContext<MatchHub> hubContext, RoomRequestDto roomRequest)
        {
            this.hubContext = hubContext;

            Name = roomRequest.LobbySettings.RoomName;
            GameMode = roomRequest.GameMode;
            Players = new Player[roomRequest.RoomCapacity];

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

        public bool TryStartMatch(string userId)
        {
            if (Status == RoomStatus.Playing)
            {
                return true;
            }

            foreach (var player in Players)
            {
                if (player == null || player.Status != PlayerStatus.Connected)
                {
                    return false;
                }
            }

            Status = RoomStatus.Playing;
            Match = new Match(GameMode, Players);

            foreach (var player in Players)
            {
                var matchData = Match.GetData(player.UserId);
                hubContext.Clients.User(player.UserId).SendAsync("MatchStarted", matchData);
            }

            return true;
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

        private void OnPlayerStatusChanged(Player player)
        {
            NotifyAllPlayers();

            if (player.Status == PlayerStatus.Connected)
            {
                return;
            }

            if (player.Status == PlayerStatus.Removed)
            {
                player.StatusChanged -= OnPlayerStatusChanged;
            }

            if (IsRoomEmpty())
            {
                Status = RoomStatus.Removing;
                StatusChanged?.Invoke(this);
            }
        }

        private void NotifyAllPlayers()
        {
            var roomData = Data;
            foreach (var player in Players)
            {
                if (player != null && player.Status != PlayerStatus.Removed)
                {
                    hubContext.Clients.User(player.UserId).SendAsync("RoomUpdated", roomData);
                }
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

            GC.SuppressFinalize(this);
        }
    }
}
