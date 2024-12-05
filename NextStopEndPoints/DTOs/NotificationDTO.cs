using NextStopEndPoints.Models;

namespace NextStopEndPoints.DTOs
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
        public NotificationTypeEnum NotificationType { get; set; }
    }
}
