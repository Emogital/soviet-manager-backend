using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public class PlayerActionDataContainer
    {
        [Key(0)]
        public PlayerActionData PlayerAction  { get; init; }
    }
}
