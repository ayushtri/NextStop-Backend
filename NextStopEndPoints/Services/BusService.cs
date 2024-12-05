using Microsoft.EntityFrameworkCore;
using NextStopEndPoints.Data;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Models;

namespace NextStopEndPoints.Services
{
    public class BusService : IBusService
    {
        private readonly NextStopDbContext _context;

        public BusService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<BusDTO> GetBusById(int busId)
        {
            try
            {
                var bus = await _context.Buses
                    .Include(b => b.Operator)
                    .FirstOrDefaultAsync(b => b.BusId == busId);

                if (bus == null)
                    return null;

                return MapToBusDTO(bus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching bus with ID {busId}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<BusDTO>> GetAllBuses()
        {
            try
            {
                var buses = await _context.Buses
                    .Include(b => b.Operator)
                    .ToListAsync();

                return buses.Select(MapToBusDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all buses: {ex.Message}");
            }
        }

        public async Task<IEnumerable<BusDTO>> GetBusesByOperatorId(int operatorId)
        {
            try
            {
                var buses = await _context.Buses
                    .Where(b => b.OperatorId == operatorId)
                    .Include(b => b.Operator)
                    .ToListAsync();

                return buses.Select(MapToBusDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching buses for operator ID {operatorId}: {ex.Message}");
            }
        }

        public async Task<BusDTO> CreateBus(CreateBusDTO createBusDTO)
        {
            try
            {
                if (!await BusNumberUnique(createBusDTO.BusNumber))
                {
                    throw new InvalidOperationException("The bus number is already in use.");
                }

                var bus = new Bus
                {
                    OperatorId = createBusDTO.OperatorId,
                    BusName = createBusDTO.BusName,
                    BusNumber = createBusDTO.BusNumber,
                    BusType = Enum.Parse<BusTypeEnum>(createBusDTO.BusType, true),
                    TotalSeats = createBusDTO.TotalSeats,
                    Amenities = createBusDTO.Amenities
                };

                await _context.Buses.AddAsync(bus);
                await _context.SaveChangesAsync();

                return MapToBusDTO(bus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating bus: {ex.Message}");
            }
        }


        public async Task<BusDTO> UpdateBus(int busId, UpdateBusDTO updateBusDTO)
        {
            try
            {
                var bus = await _context.Buses.FindAsync(busId);
                if (bus == null)
                    return null;

                if (!string.IsNullOrWhiteSpace(updateBusDTO.BusNumber) && updateBusDTO.BusNumber != bus.BusNumber)
                {
                    if (!await BusNumberUnique(updateBusDTO.BusNumber))
                    {
                        throw new InvalidOperationException("The bus number is already in use.");
                    }
                    bus.BusNumber = updateBusDTO.BusNumber;
                }

                if (!string.IsNullOrWhiteSpace(updateBusDTO.BusName))
                    bus.BusName = updateBusDTO.BusName;

                if (!string.IsNullOrWhiteSpace(updateBusDTO.BusType))
                    bus.BusType = Enum.Parse<BusTypeEnum>(updateBusDTO.BusType, true);

                if (updateBusDTO.TotalSeats > 0)
                    bus.TotalSeats = updateBusDTO.TotalSeats;

                if (!string.IsNullOrWhiteSpace(updateBusDTO.Amenities))
                    bus.Amenities = updateBusDTO.Amenities;

                _context.Buses.Update(bus);
                await _context.SaveChangesAsync();

                return MapToBusDTO(bus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating bus with ID {busId}: {ex.Message}");
            }
        }


        public async Task<BusDTO> DeleteBus(int busId)
        {
            try
            {
                var bus = await _context.Buses.FindAsync(busId);
                if (bus == null)
                    return null;

                _context.Buses.Remove(bus);
                await _context.SaveChangesAsync();

                return MapToBusDTO(bus);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting bus with ID {busId}: {ex.Message}");
            }
        }

        public async Task<bool> BusNumberUnique(string busNumber)
        {
            try
            {
                return !await _context.Buses.AnyAsync(b => b.BusNumber == busNumber);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking bus number uniqueness: {ex.Message}");
            }
        }


        private static BusDTO MapToBusDTO(Bus bus)
        {
            return new BusDTO
            {
                BusId = bus.BusId,
                OperatorId = bus.OperatorId,
                OperatorName = bus.Operator?.Name,
                BusName = bus.BusName,
                BusNumber = bus.BusNumber,
                BusType = bus.BusType.ToString(),
                TotalSeats = bus.TotalSeats,
                Amenities = bus.Amenities
            };
        }

    }
}
