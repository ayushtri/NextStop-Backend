using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Services;

namespace NextStopEndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;
        private readonly ILog _logger;

        public SeatController(ISeatService seatService, ILog logger)
        {
            _seatService = seatService;
            _logger = logger;
        }

        [HttpGet("GetSeatById/{id}")]
        public async Task<IActionResult> GetSeatById(int id)
        {
            try
            {
                var seat = await _seatService.GetSeatById(id);

                if (seat == null)
                {
                    return NotFound($"Seat with ID {id} not found.");
                }

                return Ok(seat);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching seat with ID {id}", ex);
                return StatusCode(500, "An error occurred while fetching the seat.");
            }
        }

        [HttpGet("GetAllSeats")]
        public async Task<IActionResult> GetAllSeats()
        {
            try
            {
                var seats = await _seatService.GetAllSeats();
                return Ok(seats);
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching all seats", ex);
                return StatusCode(500, "An error occurred while fetching all seats.");
            }
        }

        [HttpGet("GetSeatsByBusId/{busId}")]
        public async Task<IActionResult> GetSeatsByBusId(int busId)
        {
            try
            {
                var seats = await _seatService.GetSeatsByBusId(busId);

                if (seats == null || !seats.Any())
                {
                    return NotFound($"No seats found for bus ID {busId}.");
                }

                return Ok(seats);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching seats for bus ID {busId}", ex);
                return StatusCode(500, "An error occurred while fetching the seats for the bus.");
            }
        }

        [HttpGet("GetAvailableSeatsByBusId/{busId}")]
        public async Task<IActionResult> GetAvailableSeatsByBusId(int busId)
        {
            try
            {
                var availableSeats = await _seatService.GetAvailableSeatsByBusId(busId);

                if (availableSeats == null || !availableSeats.Any())
                {
                    return NotFound($"No available seats found for bus ID {busId}.");
                }

                return Ok(availableSeats);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching available seats for bus ID {busId}", ex);
                return StatusCode(500, "An error occurred while fetching the available seats for the bus.");
            }
        }

        [HttpPost("CreateSeatsForBus")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> CreateSeatsForBus([FromBody] CreateSeatsDTO createSeatsDTO)
        {
            try
            {
                var seats = await _seatService.CreateSeatsForBus(createSeatsDTO);

                return CreatedAtAction(nameof(GetSeatsByBusId), new { busId = createSeatsDTO.BusId }, seats);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn("Invalid operation while creating seats for bus", ex);
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating seats for bus", ex);
                return StatusCode(500, "An error occurred while creating the seats.");
            }
        }

        [HttpPut("UpdateSeatBySeatNumberAndBusId/{seatNumber}/{busId}")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> UpdateSeatBySeatNumberAndBusId(string seatNumber, int busId, [FromBody] UpdateSeatDTO updateSeatDTO)
        {
            try
            {
                var updatedSeat = await _seatService.UpdateSeatBySeatNumberAndBusId(seatNumber, busId, updateSeatDTO);

                if (updatedSeat == null)
                {
                    return NotFound($"Seat with Seat Number {seatNumber} for Bus ID {busId} not found.");
                }

                return Ok(updatedSeat);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating seat with Seat Number {seatNumber} for Bus ID {busId}", ex);
                return StatusCode(500, "An error occurred while updating the seat.");
            }
        }

        [HttpDelete("DeleteSeatBySeatNumberAndBusId/{seatNumber}/{busId}")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> DeleteSeatBySeatNumberAndBusId(string seatNumber, int busId)
        {
            try
            {
                var seat = await _seatService.DeleteSeatBySeatNumberAndBusId(seatNumber, busId);

                if (seat == null)
                {
                    return NotFound($"Seat with Seat Number {seatNumber} for Bus ID {busId} not found.");
                }

                return Ok($"Seat with Seat Number {seatNumber} for Bus ID {busId} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting seat with Seat Number {seatNumber} for Bus ID {busId}", ex);
                return StatusCode(500, "An error occurred while deleting the seat.");
            }
        }
    }
}
