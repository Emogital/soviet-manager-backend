using GameServer.Enums;
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
        public readonly bool IsMaster;

        [Key(2)]
        public readonly int OwnOrder;

        [Key(3)]
        public readonly int[] ChancesOrder;

        [Key(4)]
        public readonly int[] PenaltiesOrder;

        [Key(5)]
        public readonly PlayerData[] PlayersOrder;

        public MatchData() { }

        public MatchData(Match match, string targetUserId)
        {
            GameMode = match.GameMode;
            IsMaster = targetUserId == match.MasterId;
            ChancesOrder = match.ChancesOrder;
            PenaltiesOrder = match.PenaltiesOrder;
            PlayersOrder = new PlayerData[match.PlayersOrder.Length];

            for (int order = 0; order < match.PlayersOrder.Length; order++)
            {
                var player = match.PlayersOrder[order];
                if (player.UserId == targetUserId)
                {
                    OwnOrder = order;
                }

                PlayersOrder[order] = new PlayerData(player);
            }
        }
    }
}
