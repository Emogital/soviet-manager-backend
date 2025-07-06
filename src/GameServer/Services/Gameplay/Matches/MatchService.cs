using GameServer.Services.Gameplay.Players;
using GameServer.Services.Gameplay.Rooms;

namespace GameServer.Services.Gameplay.Matches
{
    public class MatchService(ILogger<MatchService> logger) : IMatchService
    {
        public bool TryCreateMatch(Room room, out Match? match)
        {
            match = null;

            var matchPlayers = InitMatchPlayers(room.GameMode, room.Players);
            if (matchPlayers == null)
            {
                logger.LogError("Failed to init match players for game mode {GameMode}", room.GameMode);
                return false;
            }

            match = new Match(room.GameMode, room.Theme, matchPlayers);
            return true;
        }

        private IMatchPlayer[]? InitMatchPlayers(GameMode gameMode, Player[] players)
        {
            switch (gameMode)
            {
                case GameMode.ClassicMatch:
                    return players;

                case GameMode.CoopForTwoMatch:
                    return InitCoopForTwoMatchPlayers(players);

                case GameMode.TwoByTwoMatch:
                    return InitTwoByTwoMatchPlayers(players);

                case GameMode.CoopHardcoreMatch:
                    return InitCoopHardcoreMatchPlayers(players);

                case GameMode.ConfrontationMatch:
                    return InitConfrontationMatchPlayers(players);

                case GameMode.RatingMatch:
                    return InitRatingMatchPlayers(players);

                default:
                    logger.LogWarning("Unexpected game mode {GameMode}", gameMode);
                    return null;
            }
        }

        private IMatchPlayer[] InitCoopForTwoMatchPlayers(Player[] players)
        {
            var matchPlayers = new IMatchPlayer[4];
            AddMatchPlayer(matchPlayers, players[0], playerId: 0, teamId: 1);
            AddMatchPlayer(matchPlayers, players[1], playerId: 1, teamId: 1);
            AddMatchPlayer(matchPlayers, new Bot(2), playerId: 2, teamId: 2);
            AddMatchPlayer(matchPlayers, new Bot(3), playerId: 3, teamId: 2);
            return matchPlayers;
        }

        private IMatchPlayer[] InitTwoByTwoMatchPlayers(Player[] players)
        {
            var matchPlayers = new IMatchPlayer[4];
            AddMatchPlayer(matchPlayers, players[0], playerId: 0, teamId: 1);
            AddMatchPlayer(matchPlayers, players[1], playerId: 1, teamId: 1);
            AddMatchPlayer(matchPlayers, players[2], playerId: 2, teamId: 2);
            AddMatchPlayer(matchPlayers, players[3], playerId: 3, teamId: 2);
            return matchPlayers;
        }

        private IMatchPlayer[] InitCoopHardcoreMatchPlayers(Player[] players)
        {
            var matchPlayers = new IMatchPlayer[4];
            AddMatchPlayer(matchPlayers, players[0], playerId: 0, teamId: 1);
            AddMatchPlayer(matchPlayers, players[1], playerId: 1, teamId: 1);
            AddMatchPlayer(matchPlayers, new Bot(2), playerId: 2, teamId: 2);
            AddMatchPlayer(matchPlayers, new Bot(3), playerId: 3, teamId: 2);
            AddMatchPlayer(matchPlayers, new Bot(4), playerId: 4, teamId: 2);
            return matchPlayers;
        }

        private IMatchPlayer[] InitConfrontationMatchPlayers(Player[] players)
        {
            var matchPlayers = new IMatchPlayer[4];
            AddMatchPlayer(matchPlayers, players[0], playerId: 0, teamId: 1);
            AddMatchPlayer(matchPlayers, new Bot(2), playerId: 1, teamId: 1);
            AddMatchPlayer(matchPlayers, players[1], playerId: 2, teamId: 2);
            AddMatchPlayer(matchPlayers, new Bot(3), playerId: 3, teamId: 2);
            return matchPlayers;
        }

        private IMatchPlayer[] InitRatingMatchPlayers(Player[] players)
        {
            var matchPlayers = new IMatchPlayer[4];
            AddMatchPlayer(matchPlayers, players[0], playerId: 0, teamId: 1);
            AddMatchPlayer(matchPlayers, new Bot(2), playerId: 1, teamId: 1);
            AddMatchPlayer(matchPlayers, players[1], playerId: 2, teamId: 2);
            AddMatchPlayer(matchPlayers, new Bot(3), playerId: 3, teamId: 2);
            return matchPlayers;
        }

        private void AddMatchPlayer(IMatchPlayer[] matchPlayers, IMatchPlayer player, int playerId, int teamId)
        {
            matchPlayers[playerId] = player;
            player.SetTeamId(teamId);
        }
    }
}
