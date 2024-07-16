using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SovietManager.AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;

        [HttpPost("login")]
        public IActionResult Login([FromBody] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var tokenString = GenerateJwtToken(TrimToMaxLength(userId, 36));
            return Ok(new { Token = tokenString });
        }

        private string GenerateJwtToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_SECRET_KEY"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([new Claim("sub", userId.ToString())]),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["JWT_ISSUER"],
                Audience = _configuration["JWT_AUDIENCE"],
                SigningCredentials = creds
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string TrimToMaxLength(string input, int maxLength)
        {
            return input.Length <= maxLength ? input : input.Substring(0, maxLength);
        }
    }
}
