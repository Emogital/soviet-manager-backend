using GameServer.Services.Gameplay.Matches.Actions;
using GameServer.Services.Gameplay.Players;

namespace GameServer.Services.Gameplay.Matches
{
    public class Match(GameMode gameMode, RoomTheme theme, IMatchPlayer[] players)
    {
        public readonly GameMode GameMode = gameMode;
        public readonly RoomTheme Theme = theme;
        public readonly int MasterId = 0;
        public readonly int[] ChancesOrder = ShuffleEventCards();
        public readonly int[] PenaltiesOrder = ShuffleEventCards();
        public readonly IMatchPlayer[] PlayersOrder = ShufflePlayers(players);
        public readonly List<PlayerActionData> PerformedActions = new();

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

        private static IMatchPlayer[] ShufflePlayers(IMatchPlayer[] players)
        {
            Random random = Random.Shared;
            for (int i = players.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (players[j], players[i]) = (players[i], players[j]);
            }

            return players;
        }
    }
}
