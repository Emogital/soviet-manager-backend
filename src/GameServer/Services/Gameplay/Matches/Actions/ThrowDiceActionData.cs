using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public record ThrowDiceActionData : PlayerActionData
    {
        [Key(2)]
        public DieThrowData FirstDieThrow { get; init; }
        
        [Key(3)]
        public DieThrowData SecondDieThrow { get; init; }
    }
}
