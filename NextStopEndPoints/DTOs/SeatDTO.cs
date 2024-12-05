namespace NextStopEndPoints.DTOs
{
    public class SeatDTO
    {
        public int SeatId { get; set; }
        public int BusId { get; set; }
        public string BusName { get; set; } // You may want to include the Bus name for convenience
        public string SeatNumber { get; set; }
        public bool IsAvailable { get; set; }
        public int? BookingId { get; set; }
    }
}
