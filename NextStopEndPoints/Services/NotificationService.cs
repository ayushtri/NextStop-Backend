using NextStopEndPoints.DTOs;
using NextStopEndPoints.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NextStopEndPoints.Data;

namespace NextStopEndPoints.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NextStopDbContext _context;

        public NotificationService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SendNotification(SendNotificationDTO sendNotificationDto)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = sendNotificationDto.UserId,
                    Message = sendNotificationDto.Message,
                    NotificationType = sendNotificationDto.NotificationType,
                    SentDate = DateTime.Now
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending notification: {ex.Message}");
            }
        }

        public async Task<IEnumerable<NotificationDTO>> ViewNotifications(int userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.SentDate) // Order by most recent
                    .ToListAsync();

                return notifications.Select(n => new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    UserId = n.UserId,
                    Message = n.Message,
                    SentDate = n.SentDate,
                    NotificationType = n.NotificationType
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching notifications: {ex.Message}");
            }
        }
    }
}
