using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.DTOs
{
    public class InitiatePaymentDTO
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [RegularExpression("^(successful|failed|refunded)$", ErrorMessage = "PaymentStatus must be 'successful', 'failed' or 'refunded'.")]
        public string PaymentStatus { get; set; }
    }
}
