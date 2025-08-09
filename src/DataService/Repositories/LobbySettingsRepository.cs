using DataService.Data;
using DataService.Models;
using Microsoft.EntityFrameworkCore;

namespace DataService.Repositories
{
    public class LobbySettingsRepository(DataContext context) : ILobbySettingsRepository
    {
        private readonly DataContext _context = context;

        public async Task<LobbySettingsData> GetLobbySettingsAsync(string userId)
        {
            return await _context.LobbySettings.SingleOrDefaultAsync(ls => ls.UserId == userId);
        }

        public async Task<LobbySettingsData> CreateLobbySettingsAsync(LobbySettingsData lobbySettings)
        {
            _context.LobbySettings.Add(lobbySettings);
            await _context.SaveChangesAsync();
            return lobbySettings;
        }

        public async Task<LobbySettingsData> UpdateLobbySettingsAsync(LobbySettingsData lobbySettings)
        {
            _context.LobbySettings.Update(lobbySettings);
            await _context.SaveChangesAsync();
            return lobbySettings;
        }
    }
}
