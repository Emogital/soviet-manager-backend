using GameServer.Dtos;
using GameServer.Services.Gameplay;
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

                var result = roomService.TryCreateOrJoinRoom(userId, roomRequest);
                if (result.IsSuccess)
                {
                    return Ok();
                }

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Room request failed",
                    Detail = result.ErrorCode.ToString()
                };
                problem.Extensions["errorCode"] = (int)result.ErrorCode;

                return BadRequest(problem);
            });
        }
    }
}
