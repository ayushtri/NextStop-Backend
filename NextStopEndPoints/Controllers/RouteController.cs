using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Services;

namespace NextStopEndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly ILog _logger;

        public RouteController(IRouteService routeService, ILog logger)
        {
            _routeService = routeService;
            _logger = logger;
        }

        [HttpGet("GetRouteById/{id}")]
        public async Task<IActionResult> GetRouteById(int id)
        {
            try
            {
                var route = await _routeService.GetRouteById(id);

                if (route == null)
                {
                    return NotFound($"Route with ID {id} not found.");
                }

                return Ok(route);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching route with ID {id}", ex);
                return StatusCode(500, "An error occurred while fetching the route.");
            }
        }

        [HttpGet("GetAllRoutes")]
        public async Task<IActionResult> GetAllRoutes()
        {
            try
            {
                var routes = await _routeService.GetAllRoutes();
                return Ok(routes);
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching all routes", ex);
                return StatusCode(500, "An error occurred while fetching all routes.");
            }
        }

        [HttpPost("CreateRoute")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDTO createRouteDTO)
        {
            try
            {
                var route = await _routeService.CreateRoute(createRouteDTO);
                return CreatedAtAction(nameof(GetRouteById), new { id = route.RouteId }, route);
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating route", ex);
                return StatusCode(500, "An error occurred while creating the route.");
            }
        }

        [HttpPut("UpdateRoute/{id}")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] UpdateRouteDTO updateRouteDTO)
        {
            try
            {
                var updatedRoute = await _routeService.UpdateRoute(id, updateRouteDTO);

                if (updatedRoute == null)
                {
                    return NotFound($"Route with ID {id} not found.");
                }

                return Ok(updatedRoute);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating route with ID {id}", ex);
                return StatusCode(500, "An error occurred while updating the route.");
            }
        }

        [HttpDelete("DeleteRoute/{id}")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            try
            {
                var route = await _routeService.DeleteRoute(id);

                if (route == null)
                {
                    return NotFound($"Route with ID {id} not found.");
                }

                return Ok($"Route with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting route with ID {id}", ex);
                return StatusCode(500, "An error occurred while deleting the route.");
            }
        }
    }
}
