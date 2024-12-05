using NextStopEndPoints.DTOs;

namespace NextStopEndPoints.Services
{
    public interface INotificationService
    {
        Task<bool> SendNotification(SendNotificationDTO sendNotificationDto);
        Task<IEnumerable<NotificationDTO>> ViewNotifications(int userId);
    }
}
