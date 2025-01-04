using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameServer.Hubs
{
    [Authorize(AuthenticationSchemes = "Cookie")]
    public class MatchHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userIdClaim = Context.User?.Claims.FirstOrDefault(x => x.Type == "user_id");
            var userId = userIdClaim?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"User is not available.");
            }
            else
            {
                Console.WriteLine($"User {userId} connected.");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdClaim = Context.User?.Claims.FirstOrDefault(x => x.Type == "user_id");
            var userId = userIdClaim?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"User is not available.");
            }
            else
            {
                Console.WriteLine($"User {userId} disconnected.");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
