using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [ForeignKey("Bus")]
        public int BusId { get; set; }
        public virtual Bus Bus { get; set; }

        [ForeignKey("Route")]
        public int RouteId { get; set; }
        public virtual Route Route { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Fare { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Navigation Property
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
