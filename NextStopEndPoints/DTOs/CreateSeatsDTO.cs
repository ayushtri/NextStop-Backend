using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.DTOs
{
    public class CreateSeatsDTO
    {
        [Required]
        public int BusId { get; set; }

        public List<string> SeatNumbers { get; set; }

    }
}
