using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.DTOs
{
    public class CreateBusDTO
    {
        [Required]
        public int OperatorId { get; set; }

        [StringLength(100)]
        public string BusName { get; set; }

        [Required]
        [StringLength(50)]
        public string BusNumber { get; set; }

        [Required]
        public string BusType { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TotalSeats must be greater than 0.")]
        public int TotalSeats { get; set; }

        [StringLength(255)]
        public string Amenities { get; set; }
    }
}
