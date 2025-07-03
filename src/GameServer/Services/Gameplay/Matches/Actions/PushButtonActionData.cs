using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public record PushButtonActionData : PlayerActionData
    {
        [Key(2)]
        public GameplayButton PushedButton { get; init; }
    }
}
