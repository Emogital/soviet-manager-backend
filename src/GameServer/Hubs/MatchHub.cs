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
        public async Task<RoomData?> GetRoomAsync()
        {
            var userIdClaim = Context.User?.Claims.FirstOrDefault(x => x.Type == "user_id");
            var userId = userIdClaim?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                logger.LogInformation("User identifier is not available");
                return await Task.FromException<RoomData>(new InvalidOperationException("User identifier is not available"));
            }

            if (playerService.TryGetPlayer(userId, out Player? player) == false || player == null)
            {
                logger.LogInformation("Failed to find player instance for user identifier");
                return await Task.FromException<RoomData>(new InvalidOperationException("Failed to find player instance for user identifier"));
            }

            RoomData? room = roomService.GetRoomData(player.RoomName);
            if (room == null)
            {
                logger.LogInformation("Failed to find room data for for user identifier");
                return await Task.FromException<RoomData>(new InvalidOperationException("Failed to find room data for for user identifier"));
            }

            return room;
        }

        public Task TrackHeartbeat()
        {
            var userIdClaim = Context.User?.Claims.FirstOrDefault(x => x.Type == "user_id");
            var userId = userIdClaim?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                heartbeatTracker.RecordHeartbeat(userId);
            }

            return Task.CompletedTask;
        }

        public Task<bool> TryStartMatch()
        {
            var userIdClaim = Context.User?.Claims.FirstOrDefault(x => x.Type == "user_id");
            var userId = userIdClaim?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                logger.LogInformation("User identifier is not available");
                return Task.FromException<bool>(new InvalidOperationException("User identifier is not available"));
            }

            return Task.FromResult(roomService.TryStartMatch(userId));
        }

        public override async Task OnConnectedAsync()
        {
            var userIdClaim = Context.User?.Claims.FirstOrDefault(x => x.Type == "user_id");
            var userId = userIdClaim?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                logger.LogInformation("User identifier is not available");
            }
            else
            {
                if (playerService.TryGetPlayer(userId, out Player? player) == false || player == null)
                {
                    logger.LogInformation("Failed to find player instance for user identifier");
                }
                else
                {
                    logger.LogInformation("Player {PlayerName} connected", player.Name);
                    player.ChangeStatus(PlayerStatus.Connected);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdClaim = Context.User?.Claims.FirstOrDefault(x => x.Type == "user_id");
            var userId = userIdClaim?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                logger.LogInformation("User identifier is not available");
            }
            else
            {
                if (playerService.TryGetPlayer(userId, out Player? player) == false || player == null)
                {
                    logger.LogInformation("Failed to find player instance for user identifier");
                }
                else
                {
                    logger.LogInformation("Player {PlayerName} disconnected", player.Name);
                    player.ChangeStatus(PlayerStatus.Disconnected);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
