using DataService.Dtos;

namespace DataService.Services
{
    public interface ILobbySettingsService
    {
        Task<LobbySettingsDto> GetLobbySettingsAsync(string userId);
        Task<LobbySettingsDto> CreateOrUpdateLobbySettingsAsync(string userId, LobbySettingsDto lobbySettingsDto);
    }
}
