using GameServer.Services.Gameplay.Players;
using MessagePack;

namespace GameServer.Services.Gameplay.Rooms
{
    [MessagePackObject]
    public class RoomData
    {
        [Key(0)]
        public readonly string Name;
        
        [Key(1)]
        public readonly GameMode GameMode;

        [Key(2)]
        public readonly PlayerData[] Players;

        [Key(3)]
        public readonly RoomStatus Status;

        public RoomData() { }

        public RoomData(Room room)
        {
            Name = room.Name;
            GameMode = room.GameMode;
            Status = room.Status;

            Players = new PlayerData[room.Players.Length];
            for (int i = 0; i < room.Players.Length; i++)
            {
                if (room.Players[i] == null)
                {
                    continue;
                }

                Players[i] = room.Players[i].GetData();
            }
        }
    }
}
