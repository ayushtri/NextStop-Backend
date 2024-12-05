using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.Models
{
    public class Seat
    {
        [Key]
        public int SeatId { get; set; }

        [ForeignKey("Bus")]
        public int BusId { get; set; }
        public virtual Bus Bus { get; set; }

        [Required]
        [StringLength(10)]
        public string SeatNumber { get; set; }

        public bool IsAvailable { get; set; } = true;

        [ForeignKey("Booking")]
        public int? BookingId { get; set; }
        public virtual Booking Booking { get; set; }
    }
}
