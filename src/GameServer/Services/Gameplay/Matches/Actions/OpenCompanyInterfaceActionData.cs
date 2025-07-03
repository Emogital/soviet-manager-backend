using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public record OpenCompanyInterfaceActionData : PlayerActionData
    {
        [Key(2)]
        public int ColumnNumber { get; init; }
    }
}
