namespace GameServer.Services.Gameplay.Players
{
    public interface IMatchPlayer
    {
        int Id { get; }
        int TeamId { get; }

        PlayerData GetData();
        void SetTeamId(int teamId);
    }
}
