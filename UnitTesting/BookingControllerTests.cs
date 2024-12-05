using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using NextStopEndPoints.Controllers;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using System.Linq;

namespace UnitTesting
{
    [TestFixture]
    public class BookingControllerTests
    {
        private Mock<IBookingService> _bookingServiceMock;
        private Mock<ILog> _loggerMock;
        private BookingController _controller;

        [SetUp]
        public void SetUp()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _loggerMock = new Mock<ILog>();
            _controller = new BookingController(_bookingServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task SearchBus_Success_ReturnsOk()
        {
            // Arrange
            var searchBusDto = new SearchBusDTO { Origin = "A", Destination = "B", TravelDate = System.DateTime.Now };
            var buses = new List<BusSearchResultDTO>
            {
                new BusSearchResultDTO { ScheduleId = 1, BusId = 1, BusName = "Bus A", Origin = "A", Destination = "B", DepartureTime = System.DateTime.Now, AvailableSeats = 20 }
            };
            _bookingServiceMock.Setup(s => s.SearchBus(searchBusDto)).ReturnsAsync(buses);

            // Act
            var result = await _controller.SearchBus(searchBusDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as List<BusSearchResultDTO>;
            Assert.AreEqual(buses.Count, response.Count);
        }

        [Test]
        public async Task BookTicket_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var bookTicketDto = new BookTicketDTO { UserId = 1, ScheduleId = 1, SelectedSeats = new List<string> { "A1", "A2" } };
            var booking = new BookingDTO
            {
                BookingId = 1,
                UserId = 1,
                ScheduleId = 1,
                ReservedSeats = new List<string> { "A1", "A2" },
                TotalFare = 200.0m,
                Status = "confirmed",
                BookingDate = System.DateTime.Now
            };
            _bookingServiceMock.Setup(s => s.BookTicket(bookTicketDto)).ReturnsAsync(booking);

            // Act
            var result = await _controller.BookTicket(bookTicketDto);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            var response = createdResult.Value as BookingDTO;
            Assert.AreEqual(booking.BookingId, response.BookingId);
        }

        [Test]
        public async Task CancelBooking_Success_ReturnsOk()
        {
            // Arrange
            var cancelBookingDto = new CancelBookingDTO { BookingId = 1 };
            _bookingServiceMock.Setup(s => s.CancelBooking(cancelBookingDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.CancelBooking(cancelBookingDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Booking cancelled successfully.", okResult.Value);
        }

        [Test]
        public async Task CancelBooking_BookingNotFound_ReturnsNotFound()
        {
            // Arrange
            var cancelBookingDto = new CancelBookingDTO { BookingId = 1 };
            _bookingServiceMock.Setup(s => s.CancelBooking(cancelBookingDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CancelBooking(cancelBookingDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("Booking not found or already cancelled.", notFoundResult.Value);
        }

        [Test]
        public async Task ViewBookingsByUserId_Success_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            var bookings = new List<BookingDTO>
            {
                new BookingDTO { BookingId = 1, UserId = userId, ScheduleId = 1, ReservedSeats = new List<string> { "A1" }, TotalFare = 100.0m, Status = "confirmed", BookingDate = System.DateTime.Now }
            };
            _bookingServiceMock.Setup(s => s.ViewBookingsByUserId(It.IsAny<ViewBookingsByUserIdDTO>())).ReturnsAsync(bookings);

            // Act
            var result = await _controller.ViewBookingsByUserId(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as List<BookingDTO>;
            Assert.AreEqual(bookings.Count, response.Count);
        }

        [Test]
        public async Task ViewBookingsByUserId_NoBookings_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;
            _bookingServiceMock.Setup(s => s.ViewBookingsByUserId(It.IsAny<ViewBookingsByUserIdDTO>())).ReturnsAsync(new List<BookingDTO>());

            // Act
            var result = await _controller.ViewBookingsByUserId(userId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("No bookings found for this user.", notFoundResult.Value);
        }

        [Test]
        public async Task ViewBookingsByScheduleId_Success_ReturnsOk()
        {
            // Arrange
            int scheduleId = 1;
            var bookings = new List<BookingDTO>
            {
                new BookingDTO { BookingId = 1, UserId = 1, ScheduleId = scheduleId, ReservedSeats = new List<string> { "A1" }, TotalFare = 100.0m, Status = "confirmed", BookingDate = System.DateTime.Now }
            };
            _bookingServiceMock.Setup(s => s.ViewBookingsBySchdeuleId(It.IsAny<ViewBookingsByScheduleIdDTO>())).ReturnsAsync(bookings);

            // Act
            var result = await _controller.ViewBookingsByScheduleId(scheduleId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var response = okResult.Value as List<BookingDTO>;
            Assert.AreEqual(bookings.Count, response.Count);
        }

        [Test]
        public async Task ViewBookingsByScheduleId_NoBookings_ReturnsNotFound()
        {
            // Arrange
            int scheduleId = 1;
            _bookingServiceMock.Setup(s => s.ViewBookingsBySchdeuleId(It.IsAny<ViewBookingsByScheduleIdDTO>())).ReturnsAsync(new List<BookingDTO>());

            // Act
            var result = await _controller.ViewBookingsByScheduleId(scheduleId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("No bookings found for this schedule.", notFoundResult.Value);
        }
    }
}
