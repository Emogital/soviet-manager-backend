using DataService.Dtos;
using DataService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace DataService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LobbySettingsController(ILobbySettingsService lobbySettingsService, IJwtTokenService jwtTokenService) : ControllerBase
    {
        private readonly ILobbySettingsService _lobbySettingsService = lobbySettingsService;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        [HttpGet("get")]
        [Authorize]
        public async Task<IActionResult> GetLobbySettings()
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
                var userId = _jwtTokenService.ValidateTokenAndGetUserId(token);
                var lobbySettings = await _lobbySettingsService.GetLobbySettingsAsync(userId);
                return Ok(lobbySettings);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Authorization header is missing or empty.");
            }
            catch (SecurityTokenExpiredException)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, "Token has expired.");
            }
            catch (SecurityTokenException)
            {
                return Unauthorized("Invalid or expired token.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost("update")]
        [Authorize]
        public async Task<IActionResult> UpdateLobbySettings([FromBody] LobbySettingsDto lobbySettingsDto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
                var userId = _jwtTokenService.ValidateTokenAndGetUserId(token);
                var updatedLobbySettings = await _lobbySettingsService.CreateOrUpdateLobbySettingsAsync(userId, lobbySettingsDto);
                return Ok(updatedLobbySettings);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Authorization header is missing or empty.");
            }
            catch (SecurityTokenExpiredException)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, "Token has expired.");
            }
            catch (SecurityTokenException)
            {
                return Unauthorized("Invalid or expired token.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
