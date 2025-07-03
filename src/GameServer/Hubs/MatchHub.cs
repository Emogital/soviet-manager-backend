using GameServer.Services.Gameplay.Matches.Actions;
using GameServer.Services.Gameplay.Players;
using GameServer.Services.Gameplay.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameServer.Hubs
{
    [Authorize(AuthenticationSchemes = "Cookie")]
    public class MatchHub(
        IRoomService roomService,
        IPlayerService playerService,
        IPlayerHeartbeatTracker heartbeatTracker,
        ILogger<MatchHub> logger) : Hub
    {
        private const string UserIdClaimType = "user_id";

        public override async Task OnConnectedAsync()
        {
            ExecuteWithPlayer(
                player =>
                {
                    logger.LogInformation("Player {PlayerName} connected", player.Name);
                    player.ChangeStatus(PlayerStatus.Connected);
                });

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ExecuteWithPlayer(
                player =>
                {
                    logger.LogInformation("Player {PlayerName} disconnected", player.Name);
                    player.ChangeStatus(PlayerStatus.Disconnected);
                });

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<RoomData?> GetRoomAsync()
        {
            try
            {
                var player = GetCurrentPlayer();
                var room = roomService.GetRoomData(player.RoomName);

                if (room == null)
                {
                    logger.LogWarning("Failed to find room data for player: {PlayerName}", player.Name);
                    throw new InvalidOperationException("Failed to find room data for user");
                }

                return room;
            }
            catch (InvalidOperationException)
            {
                return await Task.FromException<RoomData>(new InvalidOperationException("Unable to retrieve room data"));
            }
        }

        public Task TrackHeartbeat()
        {
            var userId = GetCurrentUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                heartbeatTracker.RecordHeartbeat(userId);
            }

            return Task.CompletedTask;
        }

        public Task<bool> TryStartMatch()
        {
            try
            {
                var userId = GetValidatedUserId();
                return Task.FromResult(roomService.TryStartMatch(userId));
            }
            catch (InvalidOperationException ex)
            {
                return Task.FromException<bool>(ex);
            }
        }

        public Task<bool> TryRegisterPlayerAction(PlayerActionDataContainer actionDataContainer)
        {
            try
            {
                var room = GetPlayerRoom(out var player);
                return Task.FromResult(room.TryRegisterPlayerAction(actionDataContainer, player.Id));
            }
            catch (InvalidOperationException)
            {
                return Task.FromException<bool>(new InvalidOperationException("Failed to register match action"));
            }
        }

        private string? GetCurrentUserId()
        {
            return Context.User?.Claims.FirstOrDefault(x => x.Type == UserIdClaimType)?.Value;
        }

        private string GetValidatedUserId()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("User identifier is not available in authentication context");
                throw new InvalidOperationException("User identifier is not available");
            }

            return userId;
        }

        private Player GetCurrentPlayer()
        {
            var userId = GetValidatedUserId();
            if (!playerService.TryGetPlayer(userId, out var player) || player == null)
            {
                logger.LogWarning("Failed to find player instance for user identifier: {UserId}", userId);
                throw new InvalidOperationException("Failed to find player instance for user identifier");
            }

            return player;
        }

        private Room GetPlayerRoom(out Player player)
        {
            player = GetCurrentPlayer();
            if (!roomService.TryGetRoom(player.RoomName, out var room) || room == null)
            {
                logger.LogWarning("Failed to find room instance for player: {PlayerName}", player.Name);
                throw new InvalidOperationException("Failed to find room instance for player");
            }

            return room;
        }

        private void ExecuteWithPlayer(Action<Player> action)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogInformation("User identifier is not available");
                return;
            }

            if (!playerService.TryGetPlayer(userId, out Player? player) || player == null)
            {
                logger.LogInformation("Failed to find player instance for user identifier");
                return;
            }

            action(player);
        }
    }
}
