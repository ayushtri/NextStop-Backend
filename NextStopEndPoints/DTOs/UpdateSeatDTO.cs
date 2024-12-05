using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.DTOs
{
    public class UpdateSeatDTO
    {
        [StringLength(10)]
        public string SeatNumber { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
