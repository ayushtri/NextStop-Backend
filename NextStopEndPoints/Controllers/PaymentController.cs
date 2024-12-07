using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Services;

namespace NextStopEndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILog _logger;

        public PaymentController(IPaymentService paymentService, ILog logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("InitiatePayment")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentDTO initiatePaymentDto)
        {
            try
            {
                var paymentStatus = await _paymentService.InitiatePayment(initiatePaymentDto);
                return CreatedAtAction(nameof(GetPaymentStatus), new { bookingId = initiatePaymentDto.BookingId }, paymentStatus);
            }
            catch (Exception ex)
            {
                _logger.Error("Error initiating payment", ex);
                return StatusCode(500, "An error occurred while initiating the payment.");
            }
        }

        [HttpGet("GetPaymentStatus/{bookingId}")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> GetPaymentStatus(int bookingId)
        {
            try
            {
                var paymentStatus = await _paymentService.GetPaymentStatus(bookingId);
                if (paymentStatus == null)
                {
                    return NotFound($"Payment status for booking ID {bookingId} not found.");
                }
                return Ok(paymentStatus);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching payment status for booking ID {bookingId}", ex);
                return StatusCode(500, "An error occurred while fetching the payment status.");
            }
        }


        [HttpPost("InitiateRefund")]
        [Authorize(Roles = "passenger,operator,admin")]
        public async Task<IActionResult> InitiateRefund([FromBody] RefundPaymentDTO refundPaymentDto)
        {
            try
            {
                var paymentStatus = await _paymentService.InitiateRefund(refundPaymentDto.BookingId);

                if (paymentStatus == null)
                {
                    return NotFound(new InitiateRefundResponseDTO { Success = false, Message = $"Refund initiation for booking ID {refundPaymentDto.BookingId} failed." });
                }

                return Ok(new InitiateRefundResponseDTO { Success = true, PaymentStatus = paymentStatus });
            }
            catch (Exception ex)
            {
                _logger.Error($"Error initiating refund for booking ID {refundPaymentDto.BookingId}", ex);
                return StatusCode(500, new InitiateRefundResponseDTO { Success = false, Message = "An error occurred while initiating the refund." });
            }
        }

    }
}
