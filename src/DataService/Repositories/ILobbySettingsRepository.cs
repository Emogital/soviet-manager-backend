using DataService.Models;

namespace DataService.Repositories
{
    public interface ILobbySettingsRepository
    {
        Task<LobbySettings> GetLobbySettingsAsync(string userId);
        Task<LobbySettings> CreateLobbySettingsAsync(LobbySettings lobbySettings);
        Task<LobbySettings> UpdateLobbySettingsAsync(LobbySettings lobbySettings);
    }
}
