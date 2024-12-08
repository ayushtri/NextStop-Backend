using NextStopEndPoints.DTOs;

namespace NextStopEndPoints.Services
{
    public interface ISeatService
    {
        Task<SeatDTO> GetSeatById(int seatId);
        Task<IEnumerable<SeatDTO>> GetAllSeats();
        Task<IEnumerable<SeatDTO>> GetSeatsByBusId(int busId);
        Task<IEnumerable<SeatDTO>> CreateSeatsForBus(CreateSeatsDTO createSeatsDTO);
        Task<SeatDTO> UpdateSeatBySeatNumberAndBusId(string seatNumber, int busId, UpdateSeatDTO updateSeatDTO);
        Task<SeatDTO> DeleteSeatBySeatNumberAndBusId(string seatNumber, int busId);
        Task<IEnumerable<SeatDTO>> GetAvailableSeatsByBusId(int busId);
        Task<IEnumerable<SeatDTO>> DeleteAllSeatsByBusId(int busId);
        Task<IEnumerable<SeatDTO>> ReleaseSeatsByBusId(int busId);
    }
}
