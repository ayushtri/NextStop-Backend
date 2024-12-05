using System;
using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.DTOs
{
    public class UpdateScheduleDTO
    {
        public int? BusId { get; set; }

        public int? RouteId { get; set; }

        public DateTime? DepartureTime { get; set; }

        public DateTime? ArrivalTime { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Fare { get; set; }

        public DateTime? Date { get; set; }
    }
}
