using MessagePack;

namespace GameServer.Services.Gameplay.Matches.Actions
{
    [MessagePackObject]
    public record EscapeVacationActionData : PlayerActionData
    {
    }
}
