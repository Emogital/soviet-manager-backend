using MessagePack;

namespace GameServer.Services.Gameplay.Players
{
    [MessagePackObject]
    public class PlayerData
    {
        [Key(0)]
        public string Name { get; init; }

        [Key(1)]
        public int Id { get; init; }

        [Key(2)]
        public int TeamId { get; init; }

        [Key(3)]
        public PlayerStatus Status { get; init; }
    }
}
