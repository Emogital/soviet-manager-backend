using System.Collections.Concurrent;

namespace GameServer.Services.Gameplay.Players
{
    public class PlayerHeartbeatTracker(ILogger<PlayerHeartbeatTracker> logger) : IPlayerHeartbeatTracker
    {
        private readonly ConcurrentDictionary<string, DateTime> lastHeartbeat = new();

        public void RecordHeartbeat(string connectionId)
        {
            lastHeartbeat[connectionId] = DateTime.UtcNow;
        }
        
        public void RemoveHeartbeat(string connectionId)
        {
            if (lastHeartbeat.TryRemove(connectionId, out _))
            {
                logger.LogInformation("Player heartbeat removed from tracker");
            }
        }

        public DateTime? GetLastHeartbeat(string connectionId)
        {
            return lastHeartbeat.TryGetValue(connectionId, out var ts) ? ts : null;
        }

        public IReadOnlyDictionary<string, DateTime> GetAllLastHeartbeats()
        {
            return lastHeartbeat;
        }
    }
}
