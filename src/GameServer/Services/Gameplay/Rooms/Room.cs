using GameServer.Dtos;
using GameServer.Hubs;
using GameServer.Services.Gameplay.Matches;
using GameServer.Services.Gameplay.Matches.Actions;
using GameServer.Services.Gameplay.Players;
using Microsoft.AspNetCore.SignalR;

namespace GameServer.Services.Gameplay.Rooms
{
    public class Room : IDisposable
    {
        private readonly IHubContext<MatchHub> hubContext;
        private readonly ILogger<RoomService> logger;

        public readonly string Name;
        public readonly GameMode GameMode;
        public readonly Player[] Players;

        public RoomStatus Status { get; private set; }
        public Match? Match { get; private set; }

        public RoomData Data => new RoomData(this);

        public event Action<Room>? StatusChanged;

        public Room(IHubContext<MatchHub> hubContext, ILogger<RoomService> logger, RoomRequestDto roomRequest)
        {
            this.hubContext = hubContext;
            this.logger = logger;

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

        public bool TryRegisterNewPlayer(string userId, string playerName)
        {
            for (int playerId = 0; playerId < Players.Length; playerId++)
            {
                if (Players[playerId] != null)
                {
                    continue;
                }

                var player = new Player(userId, playerName, Name, playerId);
                Players[playerId] = player;
                player.StatusChanged += OnPlayerStatusChanged;
                return true;
            }

            return false;
        }

        public bool TryStartMatch()
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

        public bool TryRegisterPlayerAction(PlayerActionDataContainer actionDataContainer, string connectionId)
        {
            var player = GetPlayerByConnectionId(connectionId);
            if (player == null)
            {
                logger.LogWarning("Failed to find player with connection id {ConnectionId} in room {RoomName}", connectionId, Name);
                return false;
            }

            var playerAction = actionDataContainer.PlayerAction;
            if (playerAction.ActorId != player.Id)
            {
                logger.LogWarning("Player with id {PlayerId} try to register not own action in room {RoomName}", player.Id, Name);
                return false;
            }

            if (Match is null)
            {
                logger.LogWarning("Match instance is null in room {RoomName}", Name);
                return false;
            }

            if (Match.TryRegisterPlayerAction(playerAction) == false)
            {
                logger.LogWarning("Registration of action from player with id {PlayerId} is rejected in room {RoomName}", player.Id, Name);
                return false;
            }

            hubContext.Clients.GroupExcept(Name, [connectionId]).SendAsync("PlayerActionPerformed", actionDataContainer);
            logger.LogInformation("Plater action {ActionType} for actor id {ActorId} registered in room {RoomName}", playerAction.GetType().Name, playerAction.ActorId, Name);
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

        private Player? GetPlayerByConnectionId(string connectionId)
        {
            foreach (var player in Players)
            {
                if (player != null && player.ConnectionId == connectionId)
                {
                    return player;
                }
            }

            return null;
        }

        private void OnPlayerStatusChanged(Player player)
        {
            if (player.Status == PlayerStatus.Removed)
            {
                RemovePlayer(player);
            }

            hubContext.Clients.Group(Name).SendAsync("RoomUpdated", Data);

            if (IsRoomEmpty())
            {
                Status = RoomStatus.Removing;
                StatusChanged?.Invoke(this);
            }
        }

        private void RemovePlayer(Player player)
        {
            player.StatusChanged -= OnPlayerStatusChanged;
            hubContext.Groups.RemoveFromGroupAsync(player.ConnectionId, Name);
            logger.LogInformation("Removed Player {PlayerName}", player.Name);
        }

        public void Dispose()
        {
            for (int playerId = 0; playerId < Players.Length; playerId++)
            {
                if (Players[playerId] is Player player && player.Status != PlayerStatus.Removed)
                {
                    RemovePlayer(player);
                    player.ChangeStatus(PlayerStatus.Removed);
                }

                Players[playerId] = null;
            }

            Match?.Cleanup();

            GC.SuppressFinalize(this);
        }
    }
}
