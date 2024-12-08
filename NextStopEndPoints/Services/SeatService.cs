using Microsoft.EntityFrameworkCore;
using NextStopEndPoints.Data;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextStopEndPoints.Services
{
    public class SeatService : ISeatService
    {
        private readonly NextStopDbContext _context;

        public SeatService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<SeatDTO> GetSeatById(int seatId)
        {
            var seat = await _context.Seats
                .Include(s => s.Bus)
                .FirstOrDefaultAsync(s => s.SeatId == seatId);

            if (seat == null)
                return null;

            return MapToSeatDTO(seat);
        }

        public async Task<IEnumerable<SeatDTO>> GetAllSeats()
        {
            var seats = await _context.Seats
                .Include(s => s.Bus)
                .ToListAsync();

            return seats.Select(MapToSeatDTO);
        }

        public async Task<IEnumerable<SeatDTO>> GetSeatsByBusId(int busId)
        {
            var seats = await _context.Seats
                .Where(s => s.BusId == busId)
                .Include(s => s.Bus)
                .ToListAsync();

            return seats.Select(MapToSeatDTO);
        }

        public async Task<IEnumerable<SeatDTO>> CreateSeatsForBus(CreateSeatsDTO createSeatsDTO)
        {
            try
            {
                var bus = await _context.Buses
                    .Where(b => b.BusId == createSeatsDTO.BusId)
                    .FirstOrDefaultAsync();

                if (bus == null)
                {
                    throw new InvalidOperationException($"Bus with ID {createSeatsDTO.BusId} not found.");
                }

                if (createSeatsDTO.SeatNumbers.Count > bus.TotalSeats)
                {
                    throw new InvalidOperationException($"The number of seat numbers ({createSeatsDTO.SeatNumbers.Count}) exceeds the total available seats ({bus.TotalSeats}) for the bus.");
                }


                var existingSeatNumbers = await _context.Seats
                    .Where(s => s.BusId == createSeatsDTO.BusId && createSeatsDTO.SeatNumbers.Contains(s.SeatNumber))
                    .Select(s => s.SeatNumber)
                    .ToListAsync();

                if (existingSeatNumbers.Any())
                {
                    throw new InvalidOperationException($"The following seat numbers already exist for bus ID {createSeatsDTO.BusId}: {string.Join(", ", existingSeatNumbers)}");
                }

                var seats = new List<Seat>();

                foreach (var seatNumber in createSeatsDTO.SeatNumbers)
                {
                    var seat = new Seat
                    {
                        BusId = createSeatsDTO.BusId,
                        SeatNumber = seatNumber,
                        IsAvailable = true
                    };
                    seats.Add(seat);
                }

                await _context.Seats.AddRangeAsync(seats);
                await _context.SaveChangesAsync();

                return seats.Select(MapToSeatDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating seats for bus ID {createSeatsDTO.BusId}: {ex.Message}");
            }
        }



        public async Task<SeatDTO> UpdateSeatBySeatNumberAndBusId(string seatNumber, int busId, UpdateSeatDTO updateSeatDTO)
        {
            // Find the seat with the provided seatNumber and busId
            var seat = await _context.Seats
                .FirstOrDefaultAsync(s => s.SeatNumber == seatNumber && s.BusId == busId);

            if (seat == null)
            {
                return null; // Seat not found
            }

            // Ensure SeatNumber is unique
            if (updateSeatDTO.SeatNumber != null && updateSeatDTO.SeatNumber != seat.SeatNumber)
            {
                var isSeatNumberUnique = await _context.Seats
                    .AnyAsync(s => s.SeatNumber == updateSeatDTO.SeatNumber && s.BusId == busId);

                if (isSeatNumberUnique)
                {
                    throw new InvalidOperationException("The seat number is already in use.");
                }
                seat.SeatNumber = updateSeatDTO.SeatNumber;
            }

            if (updateSeatDTO.IsAvailable.HasValue)
                seat.IsAvailable = updateSeatDTO.IsAvailable.Value;

            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();

            return MapToSeatDTO(seat);
        }


        public async Task<SeatDTO> DeleteSeatBySeatNumberAndBusId(string seatNumber, int busId)
        {
            // Find the seat with the provided seatNumber and busId
            var seat = await _context.Seats
                .FirstOrDefaultAsync(s => s.SeatNumber == seatNumber && s.BusId == busId);

            if (seat == null)
            {
                return null; // Seat not found
            }

            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();

            return MapToSeatDTO(seat);
        }



        public async Task<IEnumerable<SeatDTO>> GetAvailableSeatsByBusId(int busId)
        {
            try
            {
                var availableSeats = await _context.Seats
                    .Where(s => s.BusId == busId && s.IsAvailable)
                    .Include(s => s.Bus)
                    .ToListAsync();

                return availableSeats.Select(MapToSeatDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching available seats for bus ID {busId}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<SeatDTO>> DeleteAllSeatsByBusId(int busId)
        {
            try
            {
                var seats = await _context.Seats
                    .Where(s => s.BusId == busId)
                    .ToListAsync();

                if (!seats.Any())
                {
                    throw new InvalidOperationException($"No seats found for bus ID {busId}.");
                }

                _context.Seats.RemoveRange(seats);
                await _context.SaveChangesAsync();

                return seats.Select(MapToSeatDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting seats for bus ID {busId}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<SeatDTO>> ReleaseSeatsByBusId(int busId)
        {
            try
            {
                // Fetch all seats for the specified bus
                var seats = await _context.Seats
                    .Where(s => s.BusId == busId)
                    .ToListAsync();

                if (!seats.Any())
                {
                    throw new InvalidOperationException($"No seats found for bus ID {busId}.");
                }

                // Update IsAvailable to true and BookingId to null
                foreach (var seat in seats)
                {
                    seat.IsAvailable = true;
                    seat.BookingId = null;
                }

                // Save changes to the database
                _context.Seats.UpdateRange(seats);
                await _context.SaveChangesAsync();

                // Return updated seats as DTOs
                return seats.Select(MapToSeatDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error releasing seats for bus ID {busId}: {ex.Message}");
            }
        }

        private static SeatDTO MapToSeatDTO(Seat seat)
        {
            return new SeatDTO
            {
                SeatId = seat.SeatId,
                BusId = seat.BusId,
                BusName = seat.Bus != null ? seat.Bus.BusName : "Unknown Bus", // Mapping BusName for the response
                SeatNumber = seat.SeatNumber,
                IsAvailable = seat.IsAvailable,
                BookingId = seat.BookingId
            };
        }

    }
}
