using DataService.Data;
using DataService.Models;
using Microsoft.EntityFrameworkCore;

namespace DataService.Repositories
{
    public class LobbySettingsRepository(DataContext context) : ILobbySettingsRepository
    {
        private readonly DataContext _context = context;

        public async Task<LobbySettings> GetLobbySettingsAsync(string userId)
        {
            return await _context.LobbySettings.SingleOrDefaultAsync(ls => ls.UserId == userId);
        }

        public async Task<LobbySettings> CreateLobbySettingsAsync(LobbySettings lobbySettings)
        {
            _context.LobbySettings.Add(lobbySettings);
            await _context.SaveChangesAsync();
            return lobbySettings;
        }

        public async Task<LobbySettings> UpdateLobbySettingsAsync(LobbySettings lobbySettings)
        {
            _context.LobbySettings.Update(lobbySettings);
            await _context.SaveChangesAsync();
            return lobbySettings;
        }
    }
}
