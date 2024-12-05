using NextStopEndPoints.DTOs;

namespace NextStopEndPoints.Services
{
    public interface IBusService
    {
        Task<BusDTO> GetBusById(int busId);
        Task<IEnumerable<BusDTO>> GetAllBuses();
        Task<IEnumerable<BusDTO>> GetBusesByOperatorId(int operatorId);
        Task<BusDTO> CreateBus(CreateBusDTO createBusDTO);
        Task<BusDTO> UpdateBus(int busId, UpdateBusDTO updateBusDTO);
        Task<BusDTO> DeleteBus(int busId);
        Task<bool> BusNumberUnique(string busNumber);

    }
}
