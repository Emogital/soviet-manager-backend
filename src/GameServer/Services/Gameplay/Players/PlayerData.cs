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

        public PlayerData() { }

        public PlayerData(Player player)
        {
            Name = player.Name;
            Id = player.Id;
        }
    }
}
