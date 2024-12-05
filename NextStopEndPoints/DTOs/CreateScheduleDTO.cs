using System;
using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.DTOs
{
    public class CreateScheduleDTO
    {
        [Required]
        public int BusId { get; set; }

        [Required]
        public int RouteId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Fare { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
