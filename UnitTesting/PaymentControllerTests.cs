using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using NextStopEndPoints.Controllers;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Services;
using System.Threading.Tasks;
using log4net;

namespace UnitTesting
{
    [TestFixture]
    public class PaymentControllerTests
    {
        private Mock<IPaymentService> _paymentServiceMock;
        private Mock<ILog> _loggerMock;
        private PaymentController _controller;

        [SetUp]
        public void SetUp()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _loggerMock = new Mock<ILog>();
            _controller = new PaymentController(_paymentServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task InitiatePayment_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var initiatePaymentDto = new InitiatePaymentDTO
            {
                BookingId = 1,
                Amount = 200.0m,
                PaymentStatus = "successful"
            };

            var paymentStatus = new PaymentStatusDTO
            {
                PaymentId = 1,
                BookingId = 1,
                Amount = 200.0m,
                PaymentStatus = "successful",
                PaymentDate = System.DateTime.Now
            };

            _paymentServiceMock.Setup(p => p.InitiatePayment(initiatePaymentDto)).ReturnsAsync(paymentStatus);

            // Act
            var result = await _controller.InitiatePayment(initiatePaymentDto);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            var response = createdResult.Value as PaymentStatusDTO;
            Assert.IsNotNull(response);
            Assert.AreEqual(paymentStatus.PaymentId, response.PaymentId);
            Assert.AreEqual(paymentStatus.PaymentStatus, response.PaymentStatus);
        }

        [Test]
        public async Task GetPaymentStatus_Success_ReturnsOk()
        {
            // Arrange
            int bookingId = 1;
            var paymentStatus = new PaymentStatusDTO
            {
                PaymentId = 1,
                BookingId = bookingId,
                Amount = 200.0m,
                PaymentStatus = "successful",
                PaymentDate = System.DateTime.Now
            };

            _paymentServiceMock.Setup(p => p.GetPaymentStatus(bookingId)).ReturnsAsync(paymentStatus);

            // Act
            var result = await _controller.GetPaymentStatus(bookingId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as PaymentStatusDTO;
            Assert.IsNotNull(response);
            Assert.AreEqual(paymentStatus.PaymentId, response.PaymentId);
            Assert.AreEqual(paymentStatus.PaymentStatus, response.PaymentStatus);
        }

        [Test]
        public async Task GetPaymentStatus_NotFound_ReturnsNotFound()
        {
            // Arrange
            int bookingId = 1;

            _paymentServiceMock.Setup(p => p.GetPaymentStatus(bookingId)).ReturnsAsync((PaymentStatusDTO)null);

            // Act
            var result = await _controller.GetPaymentStatus(bookingId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult); 
            Assert.AreEqual(404, notFoundResult.StatusCode); 
            Assert.AreEqual($"Payment status for booking ID {bookingId} not found.", notFoundResult.Value); 
        }

        [Test]
        public async Task InitiateRefund_Success_ReturnsOk()
        {
            // Arrange
            int bookingId = 1;
            var refundStatus = new PaymentStatusDTO
            {
                PaymentId = 1,
                BookingId = bookingId,
                Amount = 200.0m,
                PaymentStatus = "refunded",
                PaymentDate = System.DateTime.Now
            };

            _paymentServiceMock.Setup(p => p.InitiateRefund(bookingId)).ReturnsAsync(refundStatus);

            // Act
            var result = await _controller.InitiateRefund(bookingId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as PaymentStatusDTO;
            Assert.IsNotNull(response);
            Assert.AreEqual(refundStatus.PaymentId, response.PaymentId);
            Assert.AreEqual(refundStatus.PaymentStatus, response.PaymentStatus);
        }

        [Test]
        public async Task InitiateRefund_NotFound_ReturnsNotFound()
        {
            // Arrange
            int bookingId = 1;

            _paymentServiceMock.Setup(p => p.InitiateRefund(bookingId)).ReturnsAsync((PaymentStatusDTO)null);

            // Act
            var result = await _controller.InitiateRefund(bookingId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult); 
            Assert.AreEqual(404, notFoundResult.StatusCode); 
            Assert.AreEqual($"Refund initiation for booking ID {bookingId} failed.", notFoundResult.Value); 
        }

    }
}
