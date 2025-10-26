using GameServer.Dtos;
using GameServer.Hubs;
using GameServer.Services.Gameplay.Matches;
using GameServer.Services.Gameplay.Players;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace GameServer.Services.Gameplay.Rooms
{
    public class RoomService(
        IHubContext<MatchHub> hubContext,
        IMatchService matchService,
        ILogger<RoomService> logger) : IRoomService
    {
        private readonly ConcurrentDictionary<string, Room> rooms = new();
        private readonly ConcurrentDictionary<string, string> connectionToRoom = new();

        public RoomRequestResult TryCreateOrJoinRoom(string userId, RoomRequestDto roomRequest)
        {
            if (string.IsNullOrEmpty(roomRequest.LobbySettings.RoomName))
            {
                logger.LogInformation("Failed to join room, room name is empty");
                return RoomRequestResult.Fail(RoomRequestErrorCode.RoomNameMissing);
            }

            if (string.IsNullOrEmpty(roomRequest.LobbySettings.PlayerName))
            {
                logger.LogInformation("Player name is empty, failed to join room {RoomName}", roomRequest.LobbySettings.RoomName);
                return RoomRequestResult.Fail(RoomRequestErrorCode.PlayerNameMissing);
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
                hubContext.Groups.AddToGroupAsync(connectionId, roomName);
                player.ChangeStatus(PlayerStatus.Connected);
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

        public IReadOnlyCollection<Room> GetAllRooms()
        {
            return rooms.Values.ToList().AsReadOnly();
        }

        private RoomRequestResult TryCreateRoom(string userId, RoomRequestDto roomRequest)
        {
            if (ValidateGameMode(roomRequest) == false)
            {
                return RoomRequestResult.Fail(RoomRequestErrorCode.GameModeNotAllowed);
            }
            
            if (ValidateRoomCapacity(roomRequest) == false)
            {
                return RoomRequestResult.Fail(RoomRequestErrorCode.InvalidRoomCapacity);
            }

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
                    return RoomRequestResult.Fail(RoomRequestErrorCode.RoomAlreadyExists);
                }
            }

            var room = new Room(hubContext, matchService, logger, roomRequest);
            room.StatusChanged += OnRoomStatusChanged;

            if (rooms.TryAdd(roomName, room) && room.TryRegisterNewPlayer(userId, playerName))
            {
                logger.LogInformation("Created new Room {RoomName} with game mode {GameMode}", roomName, roomRequest.GameMode);
                return RoomRequestResult.Success();
            }

            logger.LogError("Failed to create new Room {RoomName} or register Player {PlayerName}", roomName, playerName);
            RemoveRoom(room);
            return RoomRequestResult.Fail(RoomRequestErrorCode.RoomCreationOrPlayerRegistrationFailed);
        }

        private RoomRequestResult TryJoinRoom(string userId, RoomRequestDto roomRequest)
        {
            var roomName = roomRequest.LobbySettings.RoomName;
            var playerName = roomRequest.LobbySettings.PlayerName;

            if (rooms.TryGetValue(roomName, out var room) == false)
            {
                logger.LogInformation("Room {RoomName} not exists", roomName);
                return RoomRequestResult.Fail(RoomRequestErrorCode.RoomNotFound);
            }

            if (room.Status is not RoomStatus.Awaiting)
            {
                logger.LogInformation("Failed to join room {RoomName}, due room status {RoomStatus}", roomName, room.Status);
                return RoomRequestResult.Fail(RoomRequestErrorCode.RoomNotAwaiting);
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
                    return RoomRequestResult.Success();
                }

                if (player.Name == playerName)
                {
                    logger.LogInformation("Player {PlayerName} taked already in room {RoomName}", playerName, roomName);
                    return RoomRequestResult.Fail(RoomRequestErrorCode.PlayerNameAlreadyTaken);
                }
            }

            if (room.IsAvailableToJoin() == false)
            {
                logger.LogInformation("Room {RoomName} is full already, failed to join player {PlayerName}", roomName, playerName);
                return RoomRequestResult.Fail(RoomRequestErrorCode.RoomIsFull);
            }

            if (room.TryRegisterNewPlayer(userId, playerName))
            {
                return RoomRequestResult.Success();
            }

            logger.LogError("Failed to register Player {PlayerName} in Room {RoomName}", playerName, roomName);
            return RoomRequestResult.Fail(RoomRequestErrorCode.PlayerRegistrationFailed);
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

        private void OnRoomStatusChanged(Room room)
        {
            logger.LogInformation("Status of Room {room.Name} changed to {Status}", room.Name, room.Status);
            if (room.Status == RoomStatus.Removing)
            {
                RemoveRoom(room);
            }
        }

        private bool ValidateGameMode(RoomRequestDto roomRequest)
        {
            switch (roomRequest.GameMode)
            {
                case GameMode.ClassicMatch:
                    return true;

                case GameMode.CoopForTwoMatch:
                    return true;

                case GameMode.TwoByTwoMatch:
                    return true;

                case GameMode.CoopHardcoreMatch:
                    return true;

                case GameMode.ConfrontationMatch:
                    return true;

                case GameMode.RatingMatch:
                    return true;

                default:
                    logger.LogWarning("Not allowed room creation for game mode {GameMode}", roomRequest.GameMode);
                    return false;
            }
        }

        private bool ValidateRoomCapacity(RoomRequestDto roomRequest)
        {
            if (roomRequest.GameMode is GameMode.TwoByTwoMatch)
            {
                if (roomRequest.RoomCapacity != 4)
                {
                    logger.LogWarning("Room capacity in {gameMode} game mode should be 4", roomRequest.GameMode);
                    return false;
                }

                return true;
            }

            if (roomRequest.RoomCapacity < 2)
            {
                logger.LogWarning("Room capacity can't be less 2");
                return false;
            }

            if (roomRequest.GameMode is GameMode.ClassicMatch)
            {
                if (roomRequest.RoomCapacity > 5)
                {
                    logger.LogWarning("Room capacity can't be more than 5");
                    return false;
                }

                return true;
            }

            if (roomRequest.RoomCapacity > 2)
            {
                logger.LogWarning("Room capacity in {gameMode} game mode should be 2", roomRequest.GameMode);
                return false;
            }

            return true;
        }
    }
}
