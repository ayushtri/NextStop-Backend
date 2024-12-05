using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        [ForeignKey("Schedule")]
        public int ScheduleId { get; set; }
        public virtual Schedule Schedule { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalFare { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression("^(confirmed|cancelled)$", ErrorMessage = "Status must be 'confirmed' or 'cancelled'.")]
        public string Status { get; set; }

        // Navigation Property
        public virtual Payment Payment { get; set; }
        public virtual ICollection<Seat> Seats { get; set; }
    }
}
