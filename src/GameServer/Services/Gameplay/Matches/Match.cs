using GameServer.Services.Gameplay.Matches.Actions;
using GameServer.Services.Gameplay.Players;

namespace GameServer.Services.Gameplay.Matches
{
    public class Match
    {
        public readonly GameMode GameMode;
        public readonly string MasterId;
        public readonly int[] ChancesOrder;
        public readonly int[] PenaltiesOrder;
        public readonly IMatchPlayer[] PlayersOrder;
        public readonly List<PlayerActionData> PerformedActions;

        public Match(GameMode gameMode, Player[] players)
        {
            GameMode = gameMode;
            MasterId = players[0].UserId;
            ChancesOrder = ShuffleEventCards();
            PenaltiesOrder = ShuffleEventCards();
            PlayersOrder = ShufflePlayers(players);
            PerformedActions = new();
        }

        public MatchData GetData(string targetUserId)
        {
            return new MatchData(this, targetUserId);
        }

        public bool TryRegisterPlayerAction(PlayerActionData playerAction)
        {
            if (playerAction == null)
            {
                return false;
            }
        
            if (PerformedActions.Count != playerAction.ActionIndex)
            {
                return false;
            }

            PerformedActions.Add(playerAction);
            return true;
        }

        public void Cleanup()
        {
            for (int i = 0; i < PlayersOrder.Length; i++)
            {
                PlayersOrder[i] = null;
            }

            PerformedActions.Clear();
        }

        private static int[] ShuffleEventCards(int deckSize = 25)
        {
            Random random = Random.Shared;

            int[] shuffledCards = new int[deckSize];
            for (int i = 0; i < shuffledCards.Length; i++)
            {
                shuffledCards[i] = i;
            }

            for (int i = shuffledCards.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (shuffledCards[j], shuffledCards[i]) = (shuffledCards[i], shuffledCards[j]);
            }

            return shuffledCards;
        }

        private static Player[] ShufflePlayers(Player[] players)
        {
            Random random = Random.Shared;

            Player[] shuffledPlayers = new Player[players.Length];
            Array.Copy(players, shuffledPlayers, players.Length);

            for (int i = shuffledPlayers.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (shuffledPlayers[j], shuffledPlayers[i]) = (shuffledPlayers[i], shuffledPlayers[j]);
            }

            return shuffledPlayers;
        }
    }
}
