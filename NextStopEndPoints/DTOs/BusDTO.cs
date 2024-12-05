using System.Collections.Generic;

namespace NextStopEndPoints.DTOs
{
    public class BusDTO
    {
        public int BusId { get; set; }
        public int OperatorId { get; set; }
        public string OperatorName { get; set; } 
        public string BusName { get; set; }
        public string BusNumber { get; set; }
        public string BusType { get; set; }
        public int TotalSeats { get; set; }
        public string Amenities { get; set; }
    }
}
