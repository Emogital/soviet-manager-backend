using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public record OpenInventoryCardActionData : PlayerActionData
    {
        [Key(2)]
        public int CardNumber { get; init; }
    }
}
