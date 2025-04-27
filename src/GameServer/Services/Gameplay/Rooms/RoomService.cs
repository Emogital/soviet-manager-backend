using GameServer.Dtos;
using GameServer.Services.Gameplay.Players;
using System.Collections.Concurrent;

namespace GameServer.Services.Gameplay.Rooms
{
    public class RoomService(IPlayerService playerService, ILogger<RoomService> logger) : IRoomService
    {
        private readonly ConcurrentDictionary<string, Room> rooms = new();

        public bool TryCreateOrJoinRoom(string userId, RoomRequestDto roomRequest)
        {
            if (roomRequest.GameMode == 0)
            {
                return TryJoinRoom(userId, roomRequest);
            }

            return TryCreateRoom(userId, roomRequest);
        }

        private bool TryCreateRoom(string userId, RoomRequestDto roomRequest)
        {
            if (rooms.TryGetValue(roomRequest.LobbySettings.RoomName, out var existingRoom))
            {
                if (existingRoom.IsSelfRoomOrEmpty(userId))
                {
                    RemoveRoom(existingRoom);
                }
                else
                {
                    return false;
                }
            }

            var room = new Room(roomRequest);
            var player = playerService.CreatePlayer(userId, room.Name);
            room.StatusChanged += OnRoomStatusChanged;

            if (rooms.TryAdd(roomRequest.LobbySettings.RoomName, room) && room.TryRegisterPlayer(player))
            {
                logger.LogInformation("Created Room {room.Name}", room.Name);
                return true;
            }

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
                return true;
            }
            
            if (rooms.TryGetValue(roomRequest.LobbySettings.RoomName, out var room) == false || room.IsAvailableToJoin() == false)
            {
                return false;
            }

            var player = playerService.CreatePlayer(userId, room.Name);
            if (room.TryRegisterPlayer(player))
            {
                return true;
            }

            playerService.RemovePlayer(player);
            return false;
        }

        private void OnRoomStatusChanged(Room room)
        {
            if (room.Status == RoomStatus.Removing)
            {
                RemoveRoom(room);
            }
        }

        private void RemoveRoom(Room room)
        {
            if (room == null)
            {
                return;
            }

            if (rooms.TryGetValue(room.Name, out var existingRoom) && room == existingRoom)
            {
                if (rooms.TryRemove(room.Name, out _))
                {
                    logger.LogInformation("Removed Room {room.Name}", room.Name);
                }
            }

            foreach (Player? player in room.Players)
            {
                playerService.RemovePlayer(player);
            }

            room.StatusChanged -= OnRoomStatusChanged;
            room.Dispose();
        }
    }
}
