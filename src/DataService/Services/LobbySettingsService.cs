using AutoMapper;
using DataService.Dtos;
using DataService.Models;
using DataService.Repositories;

namespace DataService.Services
{
    public class LobbySettingsService(ILobbySettingsRepository repository, IMapper mapper) : ILobbySettingsService
    {
        private readonly ILobbySettingsRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<LobbySettingsDto> GetLobbySettingsAsync(string userId)
        {
            userId = TrimToMaxLength(userId, 36);
            var lobbySettings = await _repository.GetLobbySettingsAsync(userId);
            if (lobbySettings == null)
            {
                lobbySettings = new LobbySettingsData { UserId = userId };
                lobbySettings = await _repository.CreateLobbySettingsAsync(lobbySettings);
            }

            return _mapper.Map<LobbySettingsDto>(lobbySettings);
        }

        public async Task<LobbySettingsDto> CreateOrUpdateLobbySettingsAsync(string userId, LobbySettingsDto lobbySettingsDto)
        {
            userId = TrimToMaxLength(userId, 36);
            var lobbySettings = await _repository.GetLobbySettingsAsync(userId);
            if (lobbySettings == null)
            {
                lobbySettings = new LobbySettingsData
                {
                    UserId = userId,
                    RoomName = TrimToMaxLength(lobbySettingsDto.RoomName, 16),
                    PlayerName = TrimToMaxLength(lobbySettingsDto.PlayerName, 16),
                    PlayerNick = TrimToMaxLength(lobbySettingsDto.PlayerNick, 16),
                    PlayerRating = Math.Clamp(lobbySettingsDto.PlayerRating, 0, int.MaxValue)
                };
                lobbySettings = await _repository.CreateLobbySettingsAsync(lobbySettings);
            }
            else
            {
                lobbySettings.RoomName = TrimToMaxLength(lobbySettingsDto.RoomName, 16);
                lobbySettings.PlayerName = TrimToMaxLength(lobbySettingsDto.PlayerName, 16);
                lobbySettings.PlayerNick = TrimToMaxLength(lobbySettingsDto.PlayerNick, 16);
                lobbySettings.PlayerRating = Math.Clamp(lobbySettingsDto.PlayerRating, 0, int.MaxValue);
                lobbySettings = await _repository.UpdateLobbySettingsAsync(lobbySettings);
            }

            return _mapper.Map<LobbySettingsDto>(lobbySettings);
        }

        private static string TrimToMaxLength(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input.Length <= maxLength ? input : input.Substring(0, maxLength);
        }
    }
}
