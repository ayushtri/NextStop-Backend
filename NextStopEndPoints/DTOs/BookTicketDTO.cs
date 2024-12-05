namespace NextStopEndPoints.DTOs
{
    public class BookTicketDTO
    {
        public int UserId { get; set; }
        public int ScheduleId { get; set; }
        public List<string> SelectedSeats { get; set; }
    }
}
