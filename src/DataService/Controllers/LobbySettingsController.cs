using DataService.Dtos;
using DataService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LobbySettingsController(ILobbySettingsService lobbySettingsService) : IdentityControllerBase
    {
        private readonly ILobbySettingsService _lobbySettingsService = lobbySettingsService;

        [HttpGet("get")]
        [Authorize]
        public async Task<IActionResult> GetLobbySettings()
        {
            return await HandleRequestAsync(async userId =>
            {
                var lobbySettings = await _lobbySettingsService.GetLobbySettingsAsync(userId);
                return Ok(lobbySettings);
            });
        }

        [HttpPost("update")]
        [Authorize]
        public async Task<IActionResult> UpdateLobbySettings([FromBody] LobbySettingsDto lobbySettingsDto)
        {
            return await HandleRequestAsync(async userId =>
            {
                var updatedLobbySettings = await _lobbySettingsService.CreateOrUpdateLobbySettingsAsync(userId, lobbySettingsDto);
                return Ok(updatedLobbySettings);
            });
        }
    }
}
