namespace GameServer.Services.Gameplay.Players
{
    public interface IPlayerService
    {
        Player CreatePlayer(string userId, string roomName);
        bool TryGetPlayer(string userId, out Player? player);
        void RemovePlayer(Player? player);
    }
}
