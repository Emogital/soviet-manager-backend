using GameServer.Enums;
using GameServer.Models;

namespace GameServer.Dtos
{
    public class RoomResponseDto
    {
        public string RoomID { get; set; } = string.Empty;
        public GameMode GameMode { get; set; }
        public int RoomSize { get; set; }
        public PlayerData[]? Players { get; set; }
    }
}
