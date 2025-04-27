using System.Collections.Concurrent;
using GameServer.Services.Gameplay.Rooms;

namespace GameServer.Services.Gameplay.Matches
{
    public class MatchService : IMatchService
    {
        private readonly ConcurrentDictionary<Room, Match> matches = new();
    }
}
