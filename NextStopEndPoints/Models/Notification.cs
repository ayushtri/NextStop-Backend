using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextStopEndPoints.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        [StringLength(255)]
        public string Message { get; set; }

        [Required]
        public DateTime SentDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public NotificationTypeEnum NotificationType { get; set; } 
    }
    public enum NotificationTypeEnum
    {
        Email,
        SMS,
        Push
    }
}
