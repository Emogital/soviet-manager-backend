using GameServer.Dtos;
using GameServer.Hubs;
using GameServer.Services.Gameplay.Players;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace GameServer.Services.Gameplay.Rooms
{
    public class RoomService(IPlayerService playerService, IHubContext<MatchHub> hubContext, ILogger<RoomService> logger) : IRoomService
    {
        private readonly ConcurrentDictionary<string, Room> rooms = new();

        public RoomData? GetRoomData(string roomName)
        {
            if (rooms.TryGetValue(roomName, out var room) == false || room == null)
            {
                return null;
            }

            return room.Data;
        }

        public bool TryCreateOrJoinRoom(string userId, RoomRequestDto roomRequest)
        {
            if (roomRequest.GameMode == 0)
            {
                return TryJoinRoom(userId, roomRequest);
            }

            return TryCreateRoom(userId, roomRequest);
        }

        public bool TryStartMatch(string userId)
        {
            if (playerService.TryGetPlayer(userId, out Player? player) == false || player == null)
            {
                logger.LogInformation("Failed to find player instance for user identifier");
                return false;
            }

            if (rooms.TryGetValue(player.RoomName, out var room) == false || room == null)
            {
                logger.LogInformation("Failed to find room for for user identifier");
                return false;
            }

            return room.TryStartMatch(userId);
        }

        private bool TryCreateRoom(string userId, RoomRequestDto roomRequest)
        {
            if (rooms.TryGetValue(roomRequest.LobbySettings.RoomName, out var existingRoom))
            {
                if (existingRoom.IsSelfRoomOrEmpty(userId))
                {
                    logger.LogInformation("Not used Room {RoomName} found", existingRoom.Name);
                    RemoveRoom(existingRoom);
                }
                else
                {
                    logger.LogInformation("Room {RoomName} exists already", existingRoom.Name);
                    return false;
                }
            }

            var room = new Room(hubContext, roomRequest);
            var player = playerService.CreatePlayer(userId, roomRequest.LobbySettings.PlayerName, room.Name);
            room.StatusChanged += OnRoomStatusChanged;

            if (rooms.TryAdd(roomRequest.LobbySettings.RoomName, room) && room.TryRegisterPlayer(player))
            {
                logger.LogInformation("Created new Room {RoomName} with Player {PlayerName}", room.Name, player.Name);
                return true;
            }

            logger.LogInformation("Failed to register new Room {RoomName} or Player {PlayerName}", room.Name, player.Name);
            playerService.RemovePlayer(player);
            RemoveRoom(room);
            return false;
        }

        private bool TryJoinRoom(string userId, RoomRequestDto roomRequest)
        {
            if (playerService.TryGetPlayer(userId, out Player? existingPlayer) &&
                existingPlayer != null &&
                existingPlayer.RoomName == roomRequest.LobbySettings.RoomName &&
                rooms.ContainsKey(roomRequest.LobbySettings.RoomName))
            {
                logger.LogInformation("Player {PlayerName} reconnected to Room {RoomName}", existingPlayer.Name, existingPlayer.RoomName);
                return true;
            }

            if (rooms.TryGetValue(roomRequest.LobbySettings.RoomName, out var room) == false)
            {
                logger.LogInformation("Room {RoomName} not exists", roomRequest.LobbySettings.RoomName);
                return false;
            }

            if (room.IsAvailableToJoin() == false)
            {
                logger.LogInformation("Room {RoomName} is full already", room.Name);
                return false;
            }

            var player = playerService.CreatePlayer(userId, roomRequest.LobbySettings.PlayerName, room.Name);
            if (room.TryRegisterPlayer(player))
            {
                logger.LogInformation("Player {PlayerName} registered in Room {RoomName}", player.Name, room.Name);
                return true;
            }

            playerService.RemovePlayer(player);
            logger.LogInformation("Failed to register Player {PlayerName} in Room {RoomName}", player.Name, room.Name);
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
                logger.LogInformation("Attempt to remove not assigned room");
                return;
            }

            for (int playerId = 0; playerId < room.Players.Length; playerId++)
            {
                if (room.TryRemovePlayer(playerId, out Player? player))
                {
                    playerService.RemovePlayer(player);
                    logger.LogInformation("Player {PlayerName} removed from Room {RoomName}", player?.Name, room.Name);
                }
            }

            if (rooms.TryGetValue(room.Name, out var existingRoom) && room == existingRoom)
            {
                if (rooms.TryRemove(room.Name, out _))
                {
                    logger.LogInformation("Removed Room {room.Name}", room.Name);
                }
            }

            room.StatusChanged -= OnRoomStatusChanged;
            room.Dispose();
        }
    }
}
