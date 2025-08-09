using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public record AuctionBidActionData : PlayerActionData
    {
        [Key(2)]
        public int OfferedPrice { get; init; }

        [Key(3)]
        public float ScrollValue { get; init; }
    }
}
