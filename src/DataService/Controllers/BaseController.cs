using DataService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace DataService.Controllers
{
    public abstract class BaseController(IJwtTokenService jwtTokenService) : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        protected async Task<IActionResult> ProcessRequestAsync(Func<string, Task<IActionResult>> action)
        {
            try
            {
                var userId = ExtractAndValidateToken();
                return await action(userId);
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
                return Unauthorized("Invalid token.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        private string ExtractAndValidateToken()
        {
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
            return _jwtTokenService.ValidateTokenAndGetUserId(token);
        }
    }
}
