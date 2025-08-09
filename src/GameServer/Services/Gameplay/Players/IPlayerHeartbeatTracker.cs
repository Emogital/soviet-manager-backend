namespace GameServer.Services.Gameplay.Players
{
    public interface IPlayerHeartbeatTracker
    {
        void RecordHeartbeat(string userId);
        void RemoveHeartbeat(string userId);
        DateTime? GetLastHeartbeat(string userId);
        IReadOnlyDictionary<string, DateTime> GetAllLastHeartbeats();
    }
}
