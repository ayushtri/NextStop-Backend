using NextStopEndPoints.DTOs;

namespace NextStopEndPoints.Services
{
    public interface IRouteService
    {
        Task<RouteDTO> GetRouteById(int routeId);
        Task<IEnumerable<RouteDTO>> GetAllRoutes();
        Task<RouteDTO> CreateRoute(CreateRouteDTO createRouteDTO);
        Task<RouteDTO> UpdateRoute(int routeId, UpdateRouteDTO updateRouteDTO);
        Task<RouteDTO> DeleteRoute(int routeId);
    }
}
