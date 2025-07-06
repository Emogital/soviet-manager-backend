using GameServer.Services.Gameplay.Players;
using MessagePack;

namespace GameServer.Services.Gameplay.Matches
{
    [MessagePackObject]
    public class MatchData
    {
        [Key(0)]
        public readonly GameMode GameMode;

        [Key(1)]
        public readonly RoomTheme Theme;

        [Key(2)]
        public readonly int MasterId;

        [Key(3)]
        public readonly int OwnOrder;

        [Key(4)]
        public readonly int[] ChancesOrder;

        [Key(5)]
        public readonly int[] PenaltiesOrder;

        [Key(6)]
        public readonly PlayerData[] PlayersOrder;

        public MatchData() { }

        public MatchData(Match match, string targetUserId)
        {
            GameMode = match.GameMode;
            Theme = match.Theme;
            MasterId = match.MasterId;
            ChancesOrder = match.ChancesOrder;
            PenaltiesOrder = match.PenaltiesOrder;
            PlayersOrder = new PlayerData[match.PlayersOrder.Length];

            for (int order = 0; order < match.PlayersOrder.Length; order++)
            {
                var matchPlayer = match.PlayersOrder[order];
                if (matchPlayer is Player player && player.UserId == targetUserId)
                {
                    OwnOrder = order;
                }

                PlayersOrder[order] = matchPlayer.GetData();
            }
        }
    }
}
