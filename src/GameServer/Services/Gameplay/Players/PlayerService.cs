using System.Collections.Concurrent;

namespace GameServer.Services.Gameplay.Players
{
    public class PlayerService(ILogger<PlayerService> logger) : IPlayerService
    {
        private readonly ConcurrentDictionary<string, Player> players = new();

        public Player CreatePlayer(string userId, string roomName)
        {
            if (players.TryRemove(userId, out var existingPlayer))
            {
                existingPlayer.ChangeStatus(PlayerStatus.Removed);
            }

            var player = new Player(userId, roomName);
            players[userId] = player;

            logger.LogInformation("Player created with ID {PlayerId} in room {RoomName}", player.Id, roomName);

            return player;
        }

        public bool TryGetPlayer(string userId, out Player? player)
        {
            return players.TryGetValue(userId, out player);
        }

        public void RemovePlayer(Player? player)
        {
            if (player == null)
            {
                return;
            }

            if (players.TryGetValue(player.UserId, out var existingPlayer) && player == existingPlayer)
            {
                if (players.TryRemove(player.UserId,out _))
                {
                    logger.LogInformation("Removed Player {player.Id} {player.RoomName}", player.Id, player.RoomName);
                }
            }

            if (player.Status == PlayerStatus.Connected)
            {
                // notify and disconnect player
            }

            player.ChangeStatus(PlayerStatus.Removed);
        }
    }
}
