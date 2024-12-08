using NextStopEndPoints.DTOs;
using NextStopEndPoints.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NextStopEndPoints.Data;
using log4net;

namespace NextStopEndPoints.Services
{
    public class BookingService : IBookingService
    {
        private readonly NextStopDbContext _context;

        public BookingService(NextStopDbContext context)
        {
            _context = context;
        }

        // Search buses by Origin, Destination, and TravelDate
        public async Task<IEnumerable<BusSearchResultDTO>> SearchBus(SearchBusDTO searchBusDto)
        {
            try
            {
                var schedules = await _context.Schedules
                    .Include(s => s.Route)
                    .Include(s => s.Bus)
                    .Include(s => s.Bus.Seats)
                    .Where(s => s.Route.Origin == searchBusDto.Origin
                             && s.Route.Destination == searchBusDto.Destination
                             && s.DepartureTime.Date == searchBusDto.TravelDate.Date)
                    .ToListAsync();

                // Map to ScheduleDTO
                return schedules.Select(s => new BusSearchResultDTO
                {
                    ScheduleId = s.ScheduleId,
                    BusId = s.BusId,
                    RouteId = s.RouteId,
                    DepartureTime = s.DepartureTime,
                    ArrivalTime = s.ArrivalTime,
                    Fare = s.Fare,
                    Date = s.Date,
                    BusName = s.Bus?.BusName,
                    Origin = s.Route?.Origin,
                    Destination = s.Route?.Destination,
                    AvailableSeats = s.Bus?.Seats?.Count(seat => seat.IsAvailable) ?? 0
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching buses: {ex.Message}");
            }
        }

        public async Task<BookingDTO> BookTicket(BookTicketDTO bookTicketDto)
        {
            try
            {
                // Step 1: Validate the Schedule
                var schedule = await _context.Schedules
                    .Include(s => s.Bus)
                    .FirstOrDefaultAsync(s => s.ScheduleId == bookTicketDto.ScheduleId);

                if (schedule == null)
                {
                    return null;
                }


                // Step 2: Validate the User
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == bookTicketDto.UserId);

                if (user == null)
                {
                    return null;
                }

                // Step 3: Ensure the Bus has available seats
                var availableSeatsCount = await _context.Seats
                    .Where(s => s.BusId == schedule.BusId && s.IsAvailable)
                    .CountAsync();


                if (availableSeatsCount < bookTicketDto.SelectedSeats.Count)
                {
                    return null;
                }

                // Step 4: Validate Seat Availability
                var seats = await _context.Seats
                    .Where(s => bookTicketDto.SelectedSeats.Contains(s.SeatNumber) && s.BusId == schedule.BusId && s.IsAvailable)
                    .ToListAsync();


                if (seats.Count != bookTicketDto.SelectedSeats.Count)
                {
                    return null;
                }

                // Step 5: Calculate Total Fare based on Schedule Fare
                decimal totalFare = schedule.Fare * bookTicketDto.SelectedSeats.Count;


                // Step 6: Create Booking
                var booking = new Booking
                {
                    UserId = bookTicketDto.UserId,
                    ScheduleId = bookTicketDto.ScheduleId,
                    TotalFare = totalFare,
                    Status = "confirmed",
                    BookingDate = DateTime.Now
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();


                // Step 7: Mark seats as booked (not available anymore)
                foreach (var seat in seats)
                {
                    seat.IsAvailable = false;
                    seat.BookingId = booking.BookingId;
                    _context.Seats.Update(seat);
                }

                await _context.SaveChangesAsync();


                // Step 8 : Log the booked seats in SeatLog table
                var seatLog = new SeatLog
                {
                    BookingId = booking.BookingId,
                    BusId = schedule.BusId,
                    Seats = string.Join(",", bookTicketDto.SelectedSeats), 
                    DateBooked = DateTime.Now
                };
                _context.SeatLogs.Add(seatLog);
                await _context.SaveChangesAsync();


                // Step 9: Return the booking information
                return new BookingDTO
                {
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    ScheduleId = booking.ScheduleId,
                    ReservedSeats = bookTicketDto.SelectedSeats,
                    TotalFare = booking.TotalFare,  
                    Status = booking.Status,
                    BookingDate = booking.BookingDate
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error booking ticket: {ex.Message}");
            }
        }




        // Cancel a booking
        public async Task<bool> CancelBooking(CancelBookingDTO cancelBookingDTO)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Seats)
                    .FirstOrDefaultAsync(b => b.BookingId == cancelBookingDTO.BookingId);

                if (booking == null || booking.Status == "cancelled")
                {
                    return false;
                }

                // Set the booking status to 'cancelled'
                booking.Status = "cancelled";
                _context.Bookings.Update(booking);

                // Restore seat availability
                foreach (var seat in booking.Seats)
                {
                    seat.IsAvailable = true;
                    seat.BookingId = null;
                    _context.Seats.Update(seat);
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error cancelling booking: {ex.Message}");
            }
        }

        // View booking by user id
        public async Task<BookingDTO> GetBookingByBookingId(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings
                    .Where(b => b.BookingId == bookingId)
                    .Include(b => b.Seats)
                    .Include(b => b.Schedule)
                    .FirstOrDefaultAsync();

                if (booking == null)
                {
                    return null; 
                }

                return new BookingDTO
                {
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    ScheduleId = booking.ScheduleId,
                    ReservedSeats = booking.Seats.Select(s => s.SeatNumber).ToList(),
                    TotalFare = booking.TotalFare,
                    Status = booking.Status,
                    BookingDate = booking.BookingDate
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching booking with BookingId {bookingId}: {ex.Message}");
            }
        }



        // View all bookings by UserId
        public async Task<IEnumerable<BookingDTO>> ViewBookingsByUserId(ViewBookingsByUserIdDTO viewBookingsByUserIdDTO)
        {
            try
            {
                var bookings = await _context.Bookings
                    .Where(b => b.UserId == viewBookingsByUserIdDTO.UserId)
                    .Include(b => b.Schedule)
                    .Include(b => b.Seats)
                    .ToListAsync();

                if (!bookings.Any())
                {
                    return Enumerable.Empty<BookingDTO>();  // Return empty list if no bookings found
                }

                return bookings.Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    ScheduleId = b.ScheduleId,
                    ReservedSeats = b.Seats.Select(s => s.SeatNumber).ToList(),
                    TotalFare = b.TotalFare,
                    Status = b.Status,
                    BookingDate = b.BookingDate
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching bookings for user: {ex.Message}");
            }
        }


        // View all bookings by ScheduleId
        public async Task<IEnumerable<BookingDTO>> ViewBookingsBySchdeuleId(ViewBookingsByScheduleIdDTO viewBookingsByScheduleIdDTO)
        {
            try
            {
                var bookings = await _context.Bookings
                    .Where(b => b.ScheduleId == viewBookingsByScheduleIdDTO.ScheduleId)
                    .Include(b => b.User)
                    .Include(b => b.Seats)
                    .ToListAsync();

                // If no bookings are found, return an empty list
                if (!bookings.Any())
                {
                    return Enumerable.Empty<BookingDTO>(); // Return an empty list
                }

                return bookings.Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    ScheduleId = b.ScheduleId,
                    ReservedSeats = b.Seats.Select(s => s.SeatNumber).ToList(),
                    TotalFare = b.TotalFare,
                    Status = b.Status,
                    BookingDate = b.BookingDate
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching bookings for schedule: {ex.Message}");
            }
        }

        // Get SeatLog by BookingId
        public async Task<SeatLogDTO> GetSeatLogByBookingId(int bookingId)
        {
            try
            {
                // Retrieve the seat log based on BookingId
                var seatLog = await _context.SeatLogs
                    .Where(sl => sl.BookingId == bookingId)
                    .FirstOrDefaultAsync();

                if (seatLog == null)
                {
                    return null;  // Return null if no seat log is found for the given bookingId
                }

                // Map SeatLog entity to SeatLogDTO for response
                return new SeatLogDTO
                {
                    SeatLogId = seatLog.SeatLogId,
                    BookingId = seatLog.BookingId,
                    BusId = seatLog.BusId,
                    Seats = seatLog.Seats,  // Comma-separated list of booked seats
                    DateBooked = seatLog.DateBooked
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching seat log for BookingId {bookingId}: {ex.Message}");
            }
        }


    }
}
