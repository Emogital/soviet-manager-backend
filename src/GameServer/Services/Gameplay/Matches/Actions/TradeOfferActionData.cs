using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public record TradeOfferActionData : PlayerActionData
    {
        [Key(2)]
        public int TradeCompensation { get; init; }

        [Key(3)]
        public float ScrollValue { get; init; }
    }
}
