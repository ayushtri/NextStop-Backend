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
    [Authorize(Roles = "admin")] // Only admin role can access these actions
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly ILog _logger;

        public AdminDashboardController(IAdminDashboardService adminDashboardService, ILog logger)
        {
            _adminDashboardService = adminDashboardService;
            _logger = logger;
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDTO assignRoleDto)
        {
            try
            {
                var result = await _adminDashboardService.AssignRole(assignRoleDto);
                if (result)
                {
                    return Ok("Role assigned successfully.");
                }
                return BadRequest("Failed to assign role.");
            }
            catch (Exception ex)
            {
                _logger.Error("Error assigning role", ex);
                return StatusCode(500, "An error occurred while assigning the role.");
            }
        }

        [HttpPost("GenerateReports")]
        public async Task<IActionResult> GenerateReports([FromBody] GenerateReportsDTO generateReportsDto)
        {
            try
            {
                var report = await _adminDashboardService.GenerateReports(generateReportsDto);
                if (report == null || report.BookingDetails == null || !report.BookingDetails.Any())
                {
                    return NotFound("No data available for the specified criteria.");
                }
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.Error("Error generating report", ex);
                return StatusCode(500, "An error occurred while generating the report.");
            }
        }
    }
}
