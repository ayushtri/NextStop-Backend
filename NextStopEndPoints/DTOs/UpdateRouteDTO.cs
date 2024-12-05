using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.DTOs
{
    public class UpdateRouteDTO
    {
        [StringLength(100)]
        public string Origin { get; set; }

        [StringLength(100)]
        public string Destination { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Distance { get; set; }

        [StringLength(50)]
        public string EstimatedTime { get; set; }
    }
}
