using System;

namespace NextStopEndPoints.DTOs
{
    public class ScheduleDTO
    {
        public int ScheduleId { get; set; }
        public int BusId { get; set; }
        public string BusName { get; set; } // To include the bus name in the response
        public int RouteId { get; set; }
        public string Origin { get; set; } // For including origin from the Route
        public string Destination { get; set; } // For including destination from the Route
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
        public DateTime Date { get; set; }
    }
}
