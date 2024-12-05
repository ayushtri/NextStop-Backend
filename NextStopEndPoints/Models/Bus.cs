using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.Models
{
    public class Bus
    {
        [Key]
        public int BusId { get; set; }

        [Required]
        [ForeignKey("Operator")]
        public int OperatorId { get; set; }
        public virtual User Operator { get; set; } // Navigation Property

        [StringLength(100)]
        public string BusName { get; set; }

        [Required]
        [StringLength(50)]
        public string BusNumber { get; set; }

        [Required]
        public BusTypeEnum BusType { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TotalSeats must be greater than 0.")]
        public int TotalSeats { get; set; }

        [StringLength(255)]
        public string Amenities { get; set; }

        // Navigation Properties
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<Seat> Seats { get; set; }
    }

    public enum BusTypeEnum
    {
        Sleeper,
        AC,
        NonAC,
        SleeperAC,
        Seater
    }

}
