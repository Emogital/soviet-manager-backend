using GameServer.Dtos;
using GameServer.Hubs;
using GameServer.Services.Gameplay.Matches;
using GameServer.Services.Gameplay.Matches.Actions;
using GameServer.Services.Gameplay.Players;
using Microsoft.AspNetCore.SignalR;

namespace GameServer.Services.Gameplay.Rooms
{
    public class Room(
        IHubContext<MatchHub> hubContext,
        IMatchService matchService,
        ILogger<RoomService> logger,
        RoomRequestDto roomRequest) : IDisposable
    {
        public readonly string Name = roomRequest.LobbySettings.RoomName;
        public readonly GameMode GameMode = roomRequest.GameMode;
        public readonly Player[] Players = new Player[roomRequest.RoomCapacity];
        public readonly RoomTheme Theme = roomRequest.LobbySettings.Theme;

        public RoomStatus Status { get; private set; } = RoomStatus.Awaiting;

        public RoomData Data => new RoomData(this);

        private Match? match;

        public event Action<Room>? StatusChanged;

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

                var player = new Player(userId, playerName, playerId);
                Players[playerId] = player;
                player.StatusChanged += OnPlayerStatusChanged;
                logger.LogInformation("Player {PlayerName} registered in Room {RoomName}", playerName, Name);
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

            if (matchService.TryCreateMatch(this, out match) == false)
            {
                return false;
            }

            Status = RoomStatus.Playing;

            foreach (var player in Players)
            {
                var matchData = match?.GetData(player.UserId);
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

            if (match is null)
            {
                logger.LogWarning("Match instance is null in room {RoomName}", Name);
                return false;
            }

            if (match.TryRegisterPlayerAction(playerAction) == false)
            {
                logger.LogWarning("Registration of action from player with id {PlayerId} is rejected in room {RoomName}", player.Id, Name);
                return false;
            }

            hubContext.Clients.GroupExcept(Name, [connectionId]).SendAsync("PlayerActionPerformed", actionDataContainer);
            logger.LogInformation("Registered player action [{ActionIndex}] {ActionType} for actor id [{ActorId}] in room {RoomName}",
                playerAction.ActionIndex,
                playerAction.GetType().Name,
                playerAction.ActorId,
                Name);

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

            if (IsRoomEmpty())
            {
                Status = RoomStatus.Removing;
                StatusChanged?.Invoke(this);
                return;
            }

            if (Status == RoomStatus.Awaiting)
            {
                hubContext.Clients.Group(Name).SendAsync("RoomUpdated", Data);
            }

            if (Status != RoomStatus.Playing)
            {
                return;
            }

            hubContext.Clients.Group(Name).SendAsync("PlayerStatusChanged", player.GetData());

            if (match == null)
            {
                logger.LogError("Match instance is null in room {RoomName} while match playing", Name);
                return;
            }

            if (player.Status != PlayerStatus.Disconnected || player.Id != match.MasterId)
            {
                return;
            }

            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].Id == match.MasterId || Players[i].Status != PlayerStatus.Connected)
                {
                    continue;
                }

                match.MasterId = Players[i].Id;
                logger.LogInformation("New master player with id {MasterId} appointed in room {RoomName}", match.MasterId, Name);
                hubContext.Clients.Group(Name).SendAsync("MasterPlayerChanged", match.MasterId);
                return;
            }
        }

        private void RemovePlayer(Player player)
        {
            player.StatusChanged -= OnPlayerStatusChanged;
            hubContext.Groups.RemoveFromGroupAsync(player.ConnectionId, Name);
            logger.LogInformation("Removed player {PlayerName}", player.Name);
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

            match?.Cleanup();

            GC.SuppressFinalize(this);
        }
    }
}
