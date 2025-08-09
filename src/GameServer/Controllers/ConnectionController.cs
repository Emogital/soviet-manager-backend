using GameServer.Dtos;
using GameServer.Services.Gameplay.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionController(IRoomService roomService) : IdentityControllerBase
    {
        [HttpPost("joinroom")]
        [Authorize]
        public async Task<IActionResult> JoinOrCreateRoom([FromBody] RoomRequestDto roomRequest)
        {
            return await HandleRequestAsync(async userId =>
            {
                await SignInUserAsync(userId);

                if (roomRequest == null)
                {
                    return NoContent();
                }

                if (roomService.TryCreateOrJoinRoom(userId, roomRequest) == false)
                {
                    return Conflict();
                }

                return Ok();
            });
        }
    }
}
