namespace NextStopEndPoints.Models
{
    public class SeatLog
    {
        public int SeatLogId { get; set; }
        public int BookingId { get; set; }
        public int BusId { get; set; }
        public string Seats { get; set; }  
        public DateTime DateBooked { get; set; }
    }
}
