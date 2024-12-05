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
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILog _logger;

        public NotificationController(INotificationService notificationService, ILog logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpPost("SendNotification")]
        [Authorize(Roles = "operator,admin")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationDTO sendNotificationDto)
        {
            try
            {
                var result = await _notificationService.SendNotification(sendNotificationDto);
                if (result)
                {
                    return Ok("Notification sent successfully.");
                }

                return BadRequest("Failed to send notification.");
            }
            catch (Exception ex)
            {
                _logger.Error("Error sending notification", ex);
                return StatusCode(500, "An error occurred while sending the notification.");
            }
        }

        [HttpGet("ViewNotifications/{userId}")]
        [Authorize(Roles = "user,operator,admin")]
        public async Task<IActionResult> ViewNotifications(int userId)
        {
            try
            {
                var notifications = await _notificationService.ViewNotifications(userId);
                if (notifications.Any())
                {
                    return Ok(notifications);
                }

                return NotFound("No notifications found for this user.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching notifications for user ID {userId}", ex);
                return StatusCode(500, "An error occurred while fetching notifications.");
            }
        }
    }
}
