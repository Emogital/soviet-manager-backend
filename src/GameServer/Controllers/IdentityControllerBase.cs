using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GameServer.Controllers
{
    public abstract class IdentityControllerBase() : ControllerBase
    {
        protected async Task SignInUserAsync(string userId)
        {
            var claims = new[]
            {
                new Claim("user_id", userId),
            };

            var identity = new ClaimsIdentity(claims, "Cookie");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("Cookie", principal);
        }

        protected async Task<IActionResult> HandleRequestAsync(Func<string, Task<IActionResult>> action)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("User ID is missing or invalid.");
            }

            try
            {
                return await action(userId);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        private string? GetUserIdFromToken()
        {
            var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return null;
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse token: {ex.Message}");
                return null;
            }
        }
    }
}
