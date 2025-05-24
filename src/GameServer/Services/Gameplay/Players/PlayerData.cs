using MessagePack;

namespace GameServer.Services.Gameplay.Players
{
    [MessagePackObject]
    public class PlayerData
    {
        [Key(0)]
        public readonly string Name;

        [Key(1)]
        public readonly int Id;

        [Key(2)]
        public readonly PlayerStatus Status;

        public PlayerData() { }

        public PlayerData(Player player)
        {
            Name = player.Name;
            Id = player.Id;
            Status = player.Status;
        }
    }
}
