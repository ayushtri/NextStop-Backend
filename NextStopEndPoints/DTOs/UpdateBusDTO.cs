using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.DTOs
{
    public class UpdateBusDTO
    {
        [StringLength(100)]
        public string BusName { get; set; }

        [StringLength(50)]
        public string BusNumber { get; set; }

        public string BusType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "TotalSeats must be greater than 0.")]
        public int TotalSeats { get; set; }

        [StringLength(255)]
        public string Amenities { get; set; }
    }
}
