using DataService.Models;

namespace DataService.Repositories
{
    public interface ILobbySettingsRepository
    {
        Task<LobbySettingsData> GetLobbySettingsAsync(string userId);
        Task<LobbySettingsData> CreateLobbySettingsAsync(LobbySettingsData lobbySettings);
        Task<LobbySettingsData> UpdateLobbySettingsAsync(LobbySettingsData lobbySettings);
    }
}
