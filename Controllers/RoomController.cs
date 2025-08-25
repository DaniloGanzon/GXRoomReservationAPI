using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GXReservationAPI.DTOs;
using GXReservationAPI.Services;
using GXReservationAPI.Constants;
using GXReservationAPI.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using GXReservationAPI.Repository;

namespace GXReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRepostory<RoomModel> _roomService;
        private readonly IRepostory<ReservationModel> _reservationService;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRepostory<RoomModel> roomService, IRepostory<ReservationModel> reservationService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _reservationService = reservationService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAllRooms()
        {
            try
            {
                var rooms = await _roomService.GetAllAsync();
                return Ok(rooms.Select(r => new RoomDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Building = r.Building,
                    Floor = r.Floor,
                    Status = r.Status
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all rooms");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
        public async Task<ActionResult<RoomDTO>> GetRoomById(int id)
        {
            try
            {
                var room = await _roomService.GetByIdAsync(id);
                if (room == null) return NotFound();

                return Ok(new RoomDTO
                {
                    Id = room.Id,
                    Name = room.Name,
                    Building = room.Building,
                    Floor = room.Floor,
                    Status = room.Status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting room with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RoomDTO>> CreateRoom([FromBody] RoomDTO roomDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var room = new RoomModel
                {
                    Name = roomDto.Name,
                    Building = roomDto.Building,
                    Floor = roomDto.Floor,
                    Status = RoomStatus.Available // Set to Available by default
                };

                await _roomService.AddAsync(room);

                // Return all room properties in the response
                roomDto.Id = room.Id;
                roomDto.Status = room.Status;
                return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, roomDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] RoomDTO roomDto)
        {
            try
            {
                if (id != roomDto.Id) return BadRequest("ID mismatch");

                var room = await _roomService.GetByIdAsync(id);
                if (room == null) return NotFound();

                // Update all properties from the DTO
                room.Name = roomDto.Name;
                room.Building = roomDto.Building;
                room.Floor = roomDto.Floor;
                room.Status = roomDto.Status; // Make sure status is included

                await _roomService.UpdateAsync(room);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating room with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                var room = await _roomService.GetByIdAsync(id);
                if (room == null) return NotFound();

                await _roomService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting room with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}