using GameServer.Services.Gameplay.Rooms;

namespace GameServer.Services.Gameplay.Matches
{
    public interface IMatchService
    {
        bool TryCreateMatch(Room room, out Match? match);
    }
}
