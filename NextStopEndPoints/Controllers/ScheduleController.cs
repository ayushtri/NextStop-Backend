using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Services;
using System;
using System.Threading.Tasks;

namespace NextStopEndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ILog _logger;

        public ScheduleController(IScheduleService scheduleService, ILog logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }

        [HttpGet("GetScheduleById/{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            try
            {
                var schedule = await _scheduleService.GetScheduleById(id);

                if (schedule == null)
                {
                    return NotFound($"Schedule with ID {id} not found.");
                }

                return Ok(schedule);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching schedule with ID {id}", ex);
                return StatusCode(500, "An error occurred while fetching the schedule.");
            }
        }

        [HttpGet("GetAllSchedules")]
        public async Task<IActionResult> GetAllSchedules()
        {
            try
            {
                var schedules = await _scheduleService.GetAllSchedules();
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching all schedules", ex);
                return StatusCode(500, "An error occurred while fetching all schedules.");
            }
        }

  
        [HttpGet("route/{routeId}")]
        public async Task<IActionResult> GetSchedulesByRouteId(int routeId)
        {
            try
            {
                var schedules = await _scheduleService.GetSchedulesByRouteId(routeId);

                if (schedules == null)
                {
                    return NotFound($"No schedules found for route ID {routeId}.");
                }

                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching schedules for route ID {routeId}", ex);
                return StatusCode(500, "An error occurred while fetching the schedules for the route.");
            }
        }

        [HttpGet("bus/{busId}")]
        public async Task<IActionResult> GetSchedulesByBusId(int busId)
        {
            try
            {
                var schedules = await _scheduleService.GetSchedulesByBusId(busId);

                if (schedules == null)
                {
                    return NotFound($"No schedules found for bus ID {busId}.");
                }

                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching schedules for bus ID {busId}", ex);
                return StatusCode(500, "An error occurred while fetching the schedules for the bus.");
            }
        }

        [HttpPost("CreateSchedule")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDTO createScheduleDTO)
        {
            try
            {
                var schedule = await _scheduleService.CreateSchedule(createScheduleDTO);
                return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.ScheduleId }, schedule);
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating schedule", ex);
                return StatusCode(500, "An error occurred while creating the schedule.");
            }
        }

        [HttpPut("UpdateSchedule/{id}")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateScheduleDTO updateScheduleDTO)
        {
            try
            {
                var updatedSchedule = await _scheduleService.UpdateSchedule(id, updateScheduleDTO);

                if (updatedSchedule == null)
                {
                    return NotFound($"Schedule with ID {id} not found.");
                }

                return Ok(updatedSchedule);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating schedule with ID {id}", ex);
                return StatusCode(500, "An error occurred while updating the schedule.");
            }
        }

        [HttpDelete("DeleteSchedule/{id}")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            try
            {
                var schedule = await _scheduleService.DeleteSchedule(id);

                if (schedule == null)
                {
                    return NotFound($"Schedule with ID {id} not found.");
                }

                return Ok($"Schedule with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting schedule with ID {id}", ex);
                return StatusCode(500, "An error occurred while deleting the schedule.");
            }
        }
    }
}
