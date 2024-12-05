namespace NextStopEndPoints.DTOs
{
    public class GenerateReportsDTO
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Route { get; set; }
        public string Operator { get; set; }
    }
}
