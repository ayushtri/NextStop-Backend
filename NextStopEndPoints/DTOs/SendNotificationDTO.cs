using NextStopEndPoints.Models;
using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.DTOs
{
    public class SendNotificationDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Message { get; set; }

        [Required]
        public NotificationTypeEnum NotificationType { get; set; }
    }
}
