using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Services;

namespace NextStopEndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILog _logger;

        public BookingController(IBookingService bookingService, ILog logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        [HttpPost("SearchBus")]
        public async Task<IActionResult> SearchBus([FromBody] SearchBusDTO searchBusDto)
        {
            try
            {
                var buses = await _bookingService.SearchBus(searchBusDto);
                return Ok(buses);
            }
            catch (Exception ex)
            {
                _logger.Error("Error searching buses", ex);
                return StatusCode(500, "An error occurred while searching for buses.");
            }
        }

        // Book a ticket
        [HttpPost("BookTicket")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> BookTicket([FromBody] BookTicketDTO bookTicketDto)
        {
            try
            {
                var booking = await _bookingService.BookTicket(bookTicketDto);

                if (booking == null)
                {
                    return BadRequest("Unable to book ticket. Please check the provided details."); 
                }

                return CreatedAtAction(nameof(ViewBookingsByUserId), new { userId = bookTicketDto.UserId }, booking);
            }
            catch (Exception ex)
            {
                _logger.Error("Error booking ticket", ex);
                return StatusCode(500, "An error occurred while booking the ticket.");
            }
        }

        // Cancel a booking
        [HttpPost("CancelBooking")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingDTO cancelBookingDto)
        {
            try
            {
                var result = await _bookingService.CancelBooking(cancelBookingDto);

                if (result)
                {
                    return Ok(new CancelBookingResponseDTO { Success = true, Message = "Booking cancelled successfully." });
                }

                return NotFound(new CancelBookingResponseDTO { Success = false, Message = "Booking not found or already cancelled." });
            }
            catch (Exception ex)
            {
                _logger.Error("Error canceling booking", ex);
                return StatusCode(500, new CancelBookingResponseDTO { Success = false, Message = "An error occurred while canceling the booking." });
            }
        }

        [HttpGet("ViewBookingByBookingId/{bookingId}")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> ViewBookingByBookingId(int bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingByBookingId(bookingId);

                if (booking == null)
                {
                    return NotFound("Booking not found for this BookingId."); // Return 404 if booking not found
                }

                return Ok(booking); 
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching booking for BookingId {bookingId}", ex);
                return StatusCode(500, "An error occurred while fetching the booking.");
            }
        }


        [HttpGet("ViewBookingsByUserId/{userId}")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> ViewBookingsByUserId(int userId)
        {
            try
            {
                var bookings = await _bookingService.ViewBookingsByUserId(new ViewBookingsByUserIdDTO { UserId = userId });

                if (!bookings.Any()) 
                {
                    return NotFound("No bookings found for this user."); 
                }

                return Ok(bookings); 
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching bookings for user ID {userId}", ex);
                return StatusCode(500, "An error occurred while fetching bookings for the user.");
            }
        }


        [HttpGet("ViewBookingsByScheduleId/{scheduleId}")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> ViewBookingsByScheduleId(int scheduleId)
        {
            try
            {
                var bookings = await _bookingService.ViewBookingsBySchdeuleId(new ViewBookingsByScheduleIdDTO { ScheduleId = scheduleId });

                if (!bookings.Any()) // Check if no bookings exist
                {
                    return NotFound("No bookings found for this schedule."); // Return a 404 with the message
                }

                return Ok(bookings); // Return the bookings if found
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching bookings for schedule ID {scheduleId}", ex);
                return StatusCode(500, "An error occurred while fetching bookings for the schedule.");
            }
        }

    }
}
