using NextStopEndPoints.DTOs;

namespace NextStopEndPoints.Services
{
    public interface IPaymentService
    {
        Task<PaymentStatusDTO> InitiatePayment(InitiatePaymentDTO initiatePaymentDto);
        Task<PaymentStatusDTO> GetPaymentStatus(int bookingId);
        Task<PaymentStatusDTO> InitiateRefund(int bookingID);
    }
}
