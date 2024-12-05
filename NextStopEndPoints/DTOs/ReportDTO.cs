namespace NextStopEndPoints.DTOs
{
    public class ReportDTO
    {
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public string Route { get; set; }
        public string Operator { get; set; }
        public List<BookingDetailDTO> BookingDetails { get; set; }
    }
}
