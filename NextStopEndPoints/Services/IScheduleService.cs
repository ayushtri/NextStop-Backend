using NextStopEndPoints.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextStopEndPoints.Services
{
    public interface IScheduleService
    {
        Task<ScheduleDTO> GetScheduleById(int scheduleId);
        Task<IEnumerable<ScheduleDTO>> GetAllSchedules();
        Task<IEnumerable<ScheduleDTO>> GetSchedulesByRouteId(int routeId);
        Task<IEnumerable<ScheduleDTO>> GetSchedulesByBusId(int busId);
        Task<ScheduleDTO> CreateSchedule(CreateScheduleDTO createScheduleDTO);
        Task<ScheduleDTO> UpdateSchedule(int scheduleId, UpdateScheduleDTO updateScheduleDTO);
        Task<ScheduleDTO> DeleteSchedule(int scheduleId);
    }
}
