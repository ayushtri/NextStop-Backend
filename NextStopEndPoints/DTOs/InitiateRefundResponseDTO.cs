namespace NextStopEndPoints.DTOs
{
    public class InitiateRefundResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PaymentStatusDTO PaymentStatus { get; set; }
    }
}
