using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Services;

namespace NextStopEndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly IBusService _busService;
        private readonly ILog _logger;

        public BusController(IBusService busService, ILog logger)
        {
            _busService = busService;
            _logger = logger;
        }

        [HttpGet("GetBusById/{id}")]
        public async Task<IActionResult> GetBusById(int id)
        {
            try
            {
                var bus = await _busService.GetBusById(id);

                if (bus == null)
                {
                    return NotFound($"Bus with ID {id} not found.");
                }

                return Ok(bus);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching bus with ID {id}", ex);
                return StatusCode(500, "An error occurred while fetching the bus.");
            }
        }

        [HttpGet("GetAllBuses")]
        public async Task<IActionResult> GetAllBuses()
        {
            try
            {
                var buses = await _busService.GetAllBuses();
                return Ok(buses);
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching all buses", ex);
                return StatusCode(500, "An error occurred while fetching all buses.");
            }
        }

        [HttpGet("GetBusesByOperatorId/{operatorId}")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> GetBusesByOperatorId(int operatorId)
        {
            try
            {
                var buses = await _busService.GetBusesByOperatorId(operatorId);

                if (!buses.Any())
                {
                    return NotFound($"No buses found for operator ID {operatorId}.");
                }

                return Ok(buses);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching buses for operator ID {operatorId}", ex);
                return StatusCode(500, "An error occurred while fetching the buses.");
            }
        }

        [HttpPost("CreateBus")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> CreateBus([FromBody] CreateBusDTO createBusDTO)
        {
            try
            {
                var isBusNumberUnique = await _busService.BusNumberUnique(createBusDTO.BusNumber);
                if (!isBusNumberUnique)
                {
                    return BadRequest("The bus number is already in use.");
                }

                var bus = await _busService.CreateBus(createBusDTO);
                return CreatedAtAction(nameof(GetBusById), new { id = bus.BusId }, bus);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn($"Bus creation failed: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating bus", ex);
                return StatusCode(500, "An error occurred while creating the bus.");
            }
        }

        [HttpPut("UpdateBus/{id}")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> UpdateBus(int id, [FromBody] UpdateBusDTO updateBusDTO)
        {
            try
            {
                var existingBus = await _busService.GetBusById(id);
                if (existingBus == null)
                {
                    return NotFound($"Bus with ID {id} not found.");
                }

                if (!string.IsNullOrWhiteSpace(updateBusDTO.BusNumber) && updateBusDTO.BusNumber != existingBus.BusNumber)
                {
                    var isBusNumberUnique = await _busService.BusNumberUnique(updateBusDTO.BusNumber);
                    if (!isBusNumberUnique)
                    {
                        return BadRequest("The bus number is already in use.");
                    }
                }

                var updatedBus = await _busService.UpdateBus(id, updateBusDTO);

                return Ok(updatedBus);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn($"Bus update failed: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating bus with ID {id}", ex);
                return StatusCode(500, "An error occurred while updating the bus.");
            }
        }


        [HttpDelete("DeleteBus/{id}")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            try
            {
                var bus = await _busService.DeleteBus(id);

                if (bus == null)
                {
                    return NotFound($"Bus with ID {id} not found.");
                }

                return Ok($"Bus with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting bus with ID {id}", ex);
                return StatusCode(500, "An error occurred while deleting the bus.");
            }
        }
    }
}
