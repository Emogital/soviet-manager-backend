using GameServer.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionController() : IdentityControllerBase
    {
        [HttpPost("joinroom")]
        [Authorize]
        public async Task<IActionResult> JoinOrCreateRoom([FromBody] RoomRequestDto roomRequest)
        {
            return await HandleRequestAsync(async userId =>
            {
                await SignInUserAsync(userId);
                return Ok();
            });
        }
    }
}
