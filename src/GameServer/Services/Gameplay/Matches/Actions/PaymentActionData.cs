using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public record PaymentActionData : PlayerActionData
    {
        [Key(1)]
        public int Cost { get; init; }
    }
}
