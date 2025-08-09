using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    [Union(0, typeof(ThrowDiceActionData))]
    [Union(1, typeof(PushButtonActionData))]
    [Union(2, typeof(PickTradeCardActionData))]
    [Union(3, typeof(OpenInventoryCardActionData))]
    [Union(4, typeof(SelectPartnerActionData))]
    [Union(5, typeof(ClickOpenedCardActionData))]
    [Union(6, typeof(UseProtectionCardActionData))]
    [Union(7, typeof(OpenCompanyInterfaceActionData))]
    [Union(8, typeof(AuctionBidActionData))]
    [Union(9, typeof(CurrencyExchangeActionData))]
    [Union(10, typeof(TradeOfferActionData))]
    [Union(11, typeof(EscapeVacationActionData))]
    [Union(12, typeof(SurrenderActionData))]
    public abstract record PlayerActionData
    {
        [Key(0)]
        public int ActionIndex { get; init; }

        [Key(1)]
        public int ActorId { get; init; }
    }
}
