using GameServer.Services.Gameplay;
using GameServer.Services.Gameplay.Players;
using GameServer.Services.Gameplay.Rooms;
using MessagePack;

namespace GameServer.Dtos
{
    [MessagePackObject]
    public class ServerStatusDto
    {
        [Key(0)]
        public DateTime Timestamp { get; init; }

        [Key(1)]
        public int ActiveRoomsCount { get; init; }

        [Key(2)]
        public int TotalPlayersCount { get; init; }

        [Key(3)]
        public int ConnectedPlayersCount { get; init; }

        [Key(4)]
        public List<RoomStatusDto> Rooms { get; init; } = new();
    }

    [MessagePackObject]
    public class RoomStatusDto
    {
        [Key(0)]
        public string Name { get; init; } = string.Empty;

        [Key(1)]
        public GameMode GameMode { get; init; }

        [Key(2)]
        public RoomStatus Status { get; init; }

        [Key(3)]
        public int PlayerCount { get; init; }

        [Key(4)]
        public int ConnectedPlayerCount { get; init; }

        [Key(5)]
        public List<PlayerStatusDto> Players { get; init; } = new();
    }

    [MessagePackObject]
    public class PlayerStatusDto
    {
        [Key(0)]
        public string Name { get; init; } = string.Empty;

        [Key(1)]
        public PlayerStatus Status { get; init; }

        [Key(2)]
        public int Id { get; init; }

        [Key(3)]
        public int TeamId { get; init; }
    }
}
