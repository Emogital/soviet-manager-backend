using GameServer.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GameServer.Services.Gameplay.Rooms
{
    public class RoomTimerProcess(
        IHubContext<MatchHub> hubContext,
        string roomName,
        ILogger<RoomService> logger) : IDisposable
    {
        private readonly int periodicity = 30; 

        private Timer? timer;
        private int secondsPassed;
        private bool disposed;

        public void StartProcess()
        {
            //if (timer != null)
            //{
            //    return;
            //}

            //timer = new Timer(_ =>
            //{
            //    try
            //    {
            //        secondsPassed += periodicity;
            //        hubContext.Clients.Group(roomName).SendAsync("TimerTick", secondsPassed);
            //    }
            //    catch (Exception ex)
            //    {
            //        logger.LogError(ex, "Failed to send timer tick to room {RoomName}", roomName);
            //        Dispose();
            //    }
            //}, null, TimeSpan.FromSeconds(periodicity), TimeSpan.FromSeconds(periodicity));
        }

        public void Dispose()
        {
            //if (!disposed)
            //{
            //    disposed = true;

            //    timer?.Dispose();
            //    timer = null;

            //    GC.SuppressFinalize(this);
            //}
        }
    }
}