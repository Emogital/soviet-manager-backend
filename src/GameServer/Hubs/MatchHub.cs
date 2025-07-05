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
        IPlayerHeartbeatTracker heartbeatTracker,
        ILogger<MatchHub> logger) : Hub
    {
        private const string UserIdClaimType = "user_id";

        public async Task<RoomData?> ConnectToRoomAsync(string roomName)
        {
            var userId = Context.User?.Claims.FirstOrDefault(x => x.Type == UserIdClaimType)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("User identifier is not available in authentication context");
                return await Task.FromException<RoomData>(new InvalidOperationException("User identifier is not available"));
            }

            if (roomService.TryConnectToRoom(roomName, userId, Context.ConnectionId, out var room) && room != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                return room.Data;
            }

            logger.LogWarning("Failed to find room {RoomName} for user", roomName);
            return await Task.FromException<RoomData>(new InvalidOperationException("Failed to find room data for user"));
        }

        public Task TrackHeartbeat()
        {
            heartbeatTracker.RecordHeartbeat(Context.ConnectionId);
            return Task.CompletedTask;
        }

        public Task<bool> TryStartMatch()
        {
            if (roomService.TryGetConnectedRoom(Context.ConnectionId, out var room) == false || room == null)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(room.TryStartMatch());
        }

        public Task<bool> TryRegisterPlayerAction(PlayerActionDataContainer actionDataContainer)
        {
            if (roomService.TryGetConnectedRoom(Context.ConnectionId, out var room) == false || room == null)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(room.TryRegisterPlayerAction(actionDataContainer, Context.ConnectionId));
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            heartbeatTracker.RemoveHeartbeat(Context.ConnectionId);
            await roomService.DisconnectPlayer(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
