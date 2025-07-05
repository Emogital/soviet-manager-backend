using GameServer.Dtos;
using GameServer.Hubs;
using GameServer.Services.Gameplay.Players;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace GameServer.Services.Gameplay.Rooms
{
    public class RoomService(IHubContext<MatchHub> hubContext, ILogger<RoomService> logger) : IRoomService
    {
        private readonly ConcurrentDictionary<string, Room> rooms = new();
        private readonly ConcurrentDictionary<string, string> connectionToRoom = new();

        public bool TryCreateOrJoinRoom(string userId, RoomRequestDto roomRequest)
        {
            if (string.IsNullOrEmpty(roomRequest.LobbySettings.RoomName))
            {
                logger.LogInformation("Failed to join room, room name is empty");
                return false;
            }

            if (string.IsNullOrEmpty(roomRequest.LobbySettings.PlayerName))
            {
                logger.LogInformation("Player name is empty, failed to join room {RoomName}", roomRequest.LobbySettings.RoomName);
                return false;
            }

            if (roomRequest.GameMode is GameMode.None)
            {
                return TryJoinRoom(userId, roomRequest);
            }

            return TryCreateRoom(userId, roomRequest);
        }

        public bool TryConnectToRoom(string roomName, string userId, string connectionId, out Room? room)
        {
            if (rooms.TryGetValue(roomName, out room) == false || room == null)
            {
                connectionToRoom.TryRemove(connectionId, out _);
                logger.LogWarning("Failed to find room {RoomName}", roomName);
                return false;
            }

            foreach (var player in room.Players)
            {
                if (player.UserId != userId)
                {
                    continue;
                }

                connectionToRoom[connectionId] = roomName;
                player.ConnectionId = connectionId;
                player.ChangeStatus(PlayerStatus.Connected);
                hubContext.Groups.AddToGroupAsync(connectionId, roomName);
                logger.LogInformation("Player {PlayerName} connected to room {RoomName}", player.Name, roomName);
                return true;
            }

            logger.LogWarning("Room {RoomName} not contains player with user id", roomName);
            return false;
        }

        public bool TryGetConnectedRoom(string connectionId, out Room? room)
        {
            room = null;

            if (connectionToRoom.TryGetValue(connectionId, out var roomName) == false)
            {
                logger.LogWarning("Failed to find room for connection id");
                return false;
            }

            if (rooms.TryGetValue(roomName, out room) == false || room == null)
            {
                logger.LogError("Failed to find room {RoomName}", roomName);
                return false;
            }

            return true;
        }

        public async Task DisconnectPlayer(string connectionId)
        {
            if (connectionToRoom.TryRemove(connectionId, out var roomName) == false)
            {
                return;
            }

            await hubContext.Groups.RemoveFromGroupAsync(connectionId, roomName);

            if (rooms.TryGetValue(roomName, out var room) == false || room == null)
            {
                logger.LogError("Room {RoomName} for hub connection was lost", roomName);
                return;
            }

            foreach (var player in room.Players)
            {
                if (player.ConnectionId != connectionId)
                {
                    continue;
                }

                player.ChangeStatus(PlayerStatus.Disconnected);
                logger.LogInformation("Player {PlayerName} disconnected from room {RoomName}", player.Name, roomName);
                return;
            }

            logger.LogError("Room {RoomName} not contains player with connection id", roomName);
        }

        private bool TryCreateRoom(string userId, RoomRequestDto roomRequest)
        {
            var roomName = roomRequest.LobbySettings.RoomName;
            var playerName = roomRequest.LobbySettings.PlayerName;

            if (rooms.TryGetValue(roomName, out var existingRoom))
            {
                if (existingRoom.IsSelfRoomOrEmpty(userId))
                {
                    logger.LogInformation("Empty room {RoomName} found and removed", existingRoom.Name);
                    RemoveRoom(existingRoom);
                }
                else
                {
                    logger.LogInformation("Room {RoomName} exists already", existingRoom.Name);
                    return false;
                }
            }

            var room = new Room(hubContext, logger, roomRequest);
            room.StatusChanged += OnRoomStatusChanged;

            if (rooms.TryAdd(roomName, room) && room.TryRegisterNewPlayer(userId, playerName))
            {
                logger.LogInformation("Created new Room {RoomName} with Player {PlayerName}", roomName, playerName);
                return true;
            }

            logger.LogError("Failed to register new Room {RoomName} or Player {PlayerName}", roomName, playerName);
            RemoveRoom(room);
            return false;
        }

        private bool TryJoinRoom(string userId, RoomRequestDto roomRequest)
        {
            var roomName = roomRequest.LobbySettings.RoomName;
            var playerName = roomRequest.LobbySettings.PlayerName;

            if (rooms.TryGetValue(roomName, out var room) == false)
            {
                logger.LogInformation("Room {RoomName} not exists", roomName);
                return false;
            }

            if (room.Status is not RoomStatus.Awaiting)
            {
                logger.LogInformation("Failed to join room {RoomName}, due room status {RoomStatus}", roomName, room.Status);
                return false;
            }

            foreach (var player in room.Players)
            {
                if (player == null)
                {
                    continue;
                }

                if (player.UserId == userId)
                {
                    logger.LogInformation("Player {PlayerName} rejoined room {RoomName}", player.Name, roomName);
                    return true;
                }

                if (player.Name == playerName)
                {
                    logger.LogInformation("Player {PlayerName} taked already in room {RoomName}", playerName, roomName);
                    return false;
                }
            }

            if (room.IsAvailableToJoin() == false)
            {
                logger.LogInformation("Room {RoomName} is full already, failed to join player {PlayerName}", roomName, playerName);
                return false;
            }

            if (room.TryRegisterNewPlayer(userId, playerName))
            {
                logger.LogInformation("Player {PlayerName} registered in Room {RoomName}", playerName, roomName);
                return true;
            }

            logger.LogError("Failed to register Player {PlayerName} in Room {RoomName}", playerName, roomName);
            return false;
        }

        private void OnRoomStatusChanged(Room room)
        {
            logger.LogInformation("Status of Room {room.Name} changed to {Status}", room.Name, room.Status);
            if (room.Status == RoomStatus.Removing)
            {
                RemoveRoom(room);
            }
        }

        private void RemoveRoom(Room room)
        {
            if (room == null)
            {
                logger.LogError("Attempt to remove not assigned room");
                return;
            }

            if (rooms.TryRemove(room.Name, out _))
            {
                logger.LogInformation("Removed Room {room.Name}", room.Name);
            }

            room.StatusChanged -= OnRoomStatusChanged;
            room.Dispose();
        }
    }
}
