using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public record CurrencyExchangeActionData : PlayerActionData
    {
        [Key(2)]
        public int CurrencyAmount { get; init; }

        [Key(3)]
        public float ScrollValue { get; init; }
    }
}
