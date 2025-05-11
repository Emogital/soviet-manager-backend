using System.Collections.Concurrent;

namespace GameServer.Services.Gameplay.Players
{
    public class PlayerHeartbeatTracker(ILogger<PlayerHeartbeatTracker> logger) : IPlayerHeartbeatTracker
    {
        private readonly ConcurrentDictionary<string, DateTime> lastHeartbeat = new();

        public void RecordHeartbeat(string userId)
        {
            lastHeartbeat[userId] = DateTime.UtcNow;
        }
        
        public void RemoveHeartbeat(string userId)
        {
            if (lastHeartbeat.TryRemove(userId, out _))
            {
                logger.LogInformation("Player heartbeat removed from tracker");
            }
        }

        public DateTime? GetLastHeartbeat(string userId)
        {
            return lastHeartbeat.TryGetValue(userId, out var ts) ? ts : null;
        }

        public IReadOnlyDictionary<string, DateTime> GetAllLastHeartbeats()
        {
            return lastHeartbeat;
        }
    }
}
