using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SovietManager.AuthService.Services
{
    public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
    {
        private readonly IConfiguration _configuration = configuration;

        public string GenerateJwtToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_SECRET_KEY"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("sub", userId) }),
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
