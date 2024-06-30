using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SovietManager.AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;

        [HttpPost("login")]
        public IActionResult Login([FromBody] Guid userId)
        {
            if (userId != Guid.Empty)
            {
                var tokenString = GenerateJwtToken(userId);
                return Ok(new { Token = tokenString });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(Guid userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_SECRET_KEY"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([new Claim("user_id", userId.ToString())]),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["JWT_ISSUER"],
                Audience = _configuration["JWT_AUDIENCE"],
                SigningCredentials = creds
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
