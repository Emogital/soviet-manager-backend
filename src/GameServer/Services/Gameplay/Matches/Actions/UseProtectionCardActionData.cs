using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public record UseProtectionCardActionData : PlayerActionData
    {
        [Key(2)]
        public int SlotNumber { get; init; }
    }
}
