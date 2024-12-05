using NextStopEndPoints.DTOs;
using NextStopEndPoints.Models;
using NextStopEndPoints.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace NextStopEndPoints.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly NextStopDbContext _context;

        public PaymentService(NextStopDbContext context)
        {
            _context = context;
        }

        // Initiate a payment
        public async Task<PaymentStatusDTO> InitiatePayment(InitiatePaymentDTO initiatePaymentDto)
        {
            try
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == initiatePaymentDto.BookingId);

                if (booking == null)
                {
                    throw new InvalidOperationException("Booking not found.");
                }

                // Create the payment record
                var payment = new Payment
                {
                    BookingId = initiatePaymentDto.BookingId,
                    Amount = initiatePaymentDto.Amount,
                    PaymentStatus = initiatePaymentDto.PaymentStatus
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return new PaymentStatusDTO
                {
                    PaymentId = payment.PaymentId,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    PaymentStatus = payment.PaymentStatus,
                    PaymentDate = payment.PaymentDate
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error initiating payment: {ex.Message}");
            }
        }

        // Get payment status for a booking
        public async Task<PaymentStatusDTO> GetPaymentStatus(int bookingId)
        {
            try
            {
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == bookingId);

                if (payment == null)
                {
                    throw new InvalidOperationException("Payment not found.");
                }

                return new PaymentStatusDTO
                {
                    PaymentId = payment.PaymentId,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    PaymentStatus = payment.PaymentStatus,
                    PaymentDate = payment.PaymentDate
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching payment status: {ex.Message}");
            }
        }

        // Initiate a refund for a booking
        public async Task<PaymentStatusDTO> InitiateRefund(int bookingId)
        {
            try
            {
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == bookingId);

                if (payment == null)
                {
                    throw new InvalidOperationException("Payment not found.");
                }

                // Ensure only "successful" payments can be refunded
                if (payment.PaymentStatus != "successful")
                {
                    throw new InvalidOperationException("Only successful payments can be refunded.");
                }

                // Update payment status to "refunded"
                payment.PaymentStatus = "refunded";
                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();

                return new PaymentStatusDTO
                {
                    PaymentId = payment.PaymentId,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    PaymentStatus = payment.PaymentStatus,
                    PaymentDate = payment.PaymentDate
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error initiating refund: {ex.Message}");
            }
        }
    }
}
