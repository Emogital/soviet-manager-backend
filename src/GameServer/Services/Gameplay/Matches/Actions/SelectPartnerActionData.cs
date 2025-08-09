using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public record SelectPartnerActionData : PlayerActionData
    {
        [Key(2)]
        public int PartnerId { get; init; }
    }
}
