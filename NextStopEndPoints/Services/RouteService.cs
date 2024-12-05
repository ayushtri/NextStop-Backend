using Microsoft.EntityFrameworkCore;
using NextStopEndPoints.Data;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Models;

namespace NextStopEndPoints.Services
{
    public class RouteService : IRouteService
    {
        private readonly NextStopDbContext _context;

        public RouteService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<RouteDTO> GetRouteById(int routeId)
        {
            try
            {
                var route = await _context.Routes
                    .FirstOrDefaultAsync(r => r.RouteId == routeId);

                if (route == null)
                    return null;

                return MapToRouteDTO(route);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching route with ID {routeId}: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RouteDTO>> GetAllRoutes()
        {
            try
            {
                var routes = await _context.Routes.ToListAsync();
                return routes.Select(MapToRouteDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all routes: {ex.Message}");
            }
        }

        public async Task<RouteDTO> CreateRoute(CreateRouteDTO createRouteDTO)
        {
            try
            {
                var route = new Models.Route
                {
                    Origin = createRouteDTO.Origin,
                    Destination = createRouteDTO.Destination,
                    Distance = createRouteDTO.Distance,
                    EstimatedTime = createRouteDTO.EstimatedTime
                };

                await _context.Routes.AddAsync(route);
                await _context.SaveChangesAsync();

                return MapToRouteDTO(route);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating route: {ex.Message}");
            }
        }

        public async Task<RouteDTO> UpdateRoute(int routeId, UpdateRouteDTO updateRouteDTO)
        {
            try
            {
                var existingRoute = await _context.Routes.FindAsync(routeId);
                if (existingRoute == null)
                {
                    return null;
                }

                if (!string.IsNullOrWhiteSpace(updateRouteDTO.Origin))
                    existingRoute.Origin = updateRouteDTO.Origin;

                if (!string.IsNullOrWhiteSpace(updateRouteDTO.Destination))
                    existingRoute.Destination = updateRouteDTO.Destination;

                if (updateRouteDTO.Distance.HasValue)
                    existingRoute.Distance = updateRouteDTO.Distance.Value;

                if (!string.IsNullOrWhiteSpace(updateRouteDTO.EstimatedTime))
                    existingRoute.EstimatedTime = updateRouteDTO.EstimatedTime;

                _context.Routes.Update(existingRoute);
                await _context.SaveChangesAsync();

                return MapToRouteDTO(existingRoute);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating route with ID {routeId}: {ex.Message}");
            }
        }

        public async Task<RouteDTO> DeleteRoute(int routeId)
        {
            try
            {
                var route = await _context.Routes.FindAsync(routeId);
                if (route == null)
                {
                    return null;
                }

                _context.Routes.Remove(route);
                await _context.SaveChangesAsync();

                return MapToRouteDTO(route);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting route with ID {routeId}: {ex.Message}");
            }
        }

        private static RouteDTO MapToRouteDTO(Models.Route route)
        {
            return new RouteDTO
            {
                RouteId = route.RouteId,
                Origin = route.Origin,
                Destination = route.Destination,
                Distance = route.Distance,
                EstimatedTime = route.EstimatedTime
            };
        }
    }
}
