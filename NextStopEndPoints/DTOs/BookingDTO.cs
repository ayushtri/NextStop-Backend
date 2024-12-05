namespace NextStopEndPoints.DTOs
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int ScheduleId { get; set; }
        public List<string> ReservedSeats { get; set; }
        public decimal TotalFare { get; set; }
        public string Status { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
