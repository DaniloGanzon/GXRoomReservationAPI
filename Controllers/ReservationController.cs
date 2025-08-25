using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GXReservationAPI.DTOs;
using GXReservationAPI.Constants;
using GXReservationAPI.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using GXReservationAPI.Repository;

namespace GXReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReservationController : ControllerBase
    {
        private readonly IRepostory<ReservationModel> _reservationService;
        private readonly IRepostory<RoomModel> _roomService;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(IRepostory<ReservationModel> reservationService,
                                   IRepostory<RoomModel> roomService,
                                   ILogger<ReservationController> logger)
        {
            _reservationService = reservationService;
            _roomService = roomService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetAllReservations()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole(Roles.Admin);

                var reservations = await _reservationService.GetAllAsync();

                if (!isAdmin)
                {
                    reservations = reservations.Where(r => r.UserId == userId).ToList();
                }

                var reservationDtos = await MapReservationsToDtos(reservations);
                return Ok(reservationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all reservations");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservationById(int id)
        {
            try
            {
                var reservation = await _reservationService.GetByIdAsync(id);
                if (reservation == null) return NotFound();

                if (!User.IsInRole(Roles.Admin) && reservation.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return Forbid();
                }

                var reservationDto = await MapReservationToDto(reservation);
                return Ok(reservationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting reservation with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
        public async Task<ActionResult<ReservationDTO>> CreateReservation([FromBody] ReservationCreateDTO reservationDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!await _roomService.ExistsAsync(reservationDto.RoomId))
                    return BadRequest("Invalid RoomId");

                if (!await _reservationService.IsRoomAvailableAsync(
                    reservationDto.RoomId,
                    reservationDto.StartDate,
                    reservationDto.EndDate,
                    reservationDto.TimeStart,
                    reservationDto.TimeDuration))
                {
                    return BadRequest("Room is not available for the selected date/time");
                }

                var reservation = new ReservationModel
                {
                    Name = reservationDto.Name,
                    Purpose = reservationDto.Purpose,
                    RoomId = reservationDto.RoomId,
                    StartDate = reservationDto.StartDate,
                    EndDate = reservationDto.EndDate,
                    TimeStart = reservationDto.TimeStart,
                    TimeDuration = reservationDto.TimeDuration,
                    UserId = userId,
                    Status = ReservationStatus.Pending
                };

                await _reservationService.AddAsync(reservation);
                var createdDto = await MapReservationToDto(reservation);

                return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, createdDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reservation");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] ReservationDTO reservationDto)
        {
            try
            {
                if (id != reservationDto.Id) return BadRequest("ID mismatch");

                var reservation = await _reservationService.GetByIdAsync(id);
                if (reservation == null) return NotFound();

                if (!User.IsInRole(Roles.Admin) && reservation.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return Forbid();
                }

                if (reservationDto.RoomId != reservation.RoomId &&
                    !await _roomService.ExistsAsync(reservationDto.RoomId))
                {
                    return BadRequest("Invalid RoomId");
                }

                if (User.IsInRole(Roles.Admin) && !string.IsNullOrEmpty(reservationDto.Status))
                {
                    reservation.Status = reservationDto.Status;
                }

                reservation.Name = reservationDto.Name;
                reservation.Purpose = reservationDto.Purpose;
                reservation.RoomId = reservationDto.RoomId;
                reservation.StartDate = reservationDto.StartDate;
                reservation.EndDate = reservationDto.EndDate;
                reservation.TimeStart = reservationDto.TimeStart;
                reservation.TimeDuration = reservationDto.TimeDuration;

                await _reservationService.UpdateAsync(reservation);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating reservation with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/approve")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ApproveReservation(int id)
        {
            return await UpdateReservationStatus(id, ReservationStatus.Approved);
        }

        [HttpPatch("{id}/reject")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RejectReservation(int id)
        {
            return await UpdateReservationStatus(id, ReservationStatus.Rejected);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                var reservation = await _reservationService.GetByIdAsync(id);
                if (reservation == null) return NotFound();

                if (!User.IsInRole(Roles.Admin) && reservation.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return Forbid();
                }

                await _reservationService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting reservation with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("room/{roomId}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservationsByRoom(int roomId)
        {
            try
            {
                var reservations = (await _reservationService.GetByRoomIdAsync(roomId))
                    .Where(r => r.Status != ReservationStatus.Rejected)
                    .ToList();

                var room = await _roomService.GetByIdAsync(roomId);
                var roomDto = room != null ? MapRoomToDto(room) : null;

                var reservationDtos = new List<ReservationDTO>();
                foreach (var reservation in reservations)
                {
                    reservationDtos.Add(await MapReservationToDto(reservation, roomDto));
                }

                return Ok(reservationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting reservations for room {roomId}");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<List<ReservationDTO>> MapReservationsToDtos(IEnumerable<ReservationModel> reservations)
        {
            var dtos = new List<ReservationDTO>();
            foreach (var reservation in reservations)
            {
                dtos.Add(await MapReservationToDto(reservation));
            }
            return dtos;
        }

        private async Task<ReservationDTO> MapReservationToDto(ReservationModel reservation, RoomDTO? roomDto = null)
        {
            var room = roomDto ?? MapRoomToDto(await _roomService.GetByIdAsync(reservation.RoomId));

            return new ReservationDTO
            {
                Id = reservation.Id,
                Name = reservation.Name,
                Purpose = reservation.Purpose,
                RoomId = reservation.RoomId,
                Room = room,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                TimeStart = reservation.TimeStart,
                TimeDuration = reservation.TimeDuration,
                Status = reservation.Status
            };
        }

        private async Task<IActionResult> UpdateReservationStatus(int id, string status)
        {
            try
            {
                var reservation = await _reservationService.GetByIdAsync(id);
                if (reservation == null) return NotFound();

                reservation.Status = status;
                await _reservationService.UpdateAsync(reservation);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating reservation status for id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        private static RoomDTO? MapRoomToDto(RoomModel? room)
        {
            return room != null ? new RoomDTO
            {
                Id = room.Id,
                Name = room.Name,
                Building = room.Building,
                Floor = room.Floor,
                Status = room.Status
            } : null;
        }
    }
}