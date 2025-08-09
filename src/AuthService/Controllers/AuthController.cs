using Microsoft.AspNetCore.Mvc;
using SovietManager.AuthService.Services;

namespace SovietManager.AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IJwtTokenService jwtTokenService) : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        [HttpPost("login")]
        public IActionResult Login([FromBody] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var tokenString = _jwtTokenService.GenerateJwtToken(TrimToMaxLength(userId, 36));
            return Ok(new { Token = tokenString });
        }

        private static string TrimToMaxLength(string input, int maxLength)
        {
            return input.Length <= maxLength ? input : input.Substring(0, maxLength);
        }
    }
}
