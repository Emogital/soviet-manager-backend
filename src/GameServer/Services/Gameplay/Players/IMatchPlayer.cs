namespace GameServer.Services.Gameplay.Players
{
    public interface IMatchPlayer
    {
        string Name { get; } 
        int Id { get; }
        int TeamId { get; }
        public PlayerStatus Status { get; }

        PlayerData GetData();
        void SetTeamId(int teamId);
    }
}
