using GameServer.Dtos;
using GameServer.Services.Gameplay.Players;
using GameServer.Services.Gameplay.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController(IRoomService roomService, ILogger<AdminController> logger) : ControllerBase
    {
        [HttpGet("server-status")]
        public IActionResult GetServerStatus()
        {
            try
            {
                var rooms = roomService.GetAllRooms();
                var roomsList = new List<RoomStatusDto>();

                int totalPlayers = 0;
                int connectedPlayers = 0;

                foreach (var room in rooms)
                {
                    var roomPlayers = new List<PlayerStatusDto>();
                    int roomPlayerCount = 0;
                    int roomConnectedCount = 0;

                    foreach (var player in room.Players)
                    {
                        if (player == null) continue;

                        roomPlayerCount++;
                        totalPlayers++;

                        if (player.Status == PlayerStatus.Connected)
                        {
                            roomConnectedCount++;
                            connectedPlayers++;
                        }

                        roomPlayers.Add(new PlayerStatusDto
                        {
                            Name = player.Name,
                            Status = player.Status,
                            Id = player.Id,
                            TeamId = player.TeamId
                        });
                    }

                    roomsList.Add(new RoomStatusDto
                    {
                        Name = room.Name,
                        GameMode = room.GameMode,
                        Status = room.Status,
                        PlayerCount = roomPlayerCount,
                        ConnectedPlayerCount = roomConnectedCount,
                        Players = roomPlayers
                    });
                }

                var response = new ServerStatusDto
                {
                    Timestamp = DateTime.UtcNow,
                    ActiveRoomsCount = rooms.Count,
                    TotalPlayersCount = totalPlayers,
                    ConnectedPlayersCount = connectedPlayers,
                    Rooms = roomsList
                };

                logger.LogInformation("Server status requested - {ActiveRooms} rooms, {TotalPlayers} total players, {ConnectedPlayers} connected", 
                    rooms.Count, totalPlayers, connectedPlayers);

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving server status");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
