using Moq;
using NUnit.Framework;
using NextStopEndPoints.Controllers;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Models;
using NextStopEndPoints.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace UnitTesting
{
    [TestFixture]
    public class SeatControllerTests
    {
        private Mock<ISeatService> _seatServiceMock;
        private Mock<ILog> _loggerMock;
        private SeatController _controller;

        [SetUp]
        public void SetUp()
        {
            _seatServiceMock = new Mock<ISeatService>();
            _loggerMock = new Mock<ILog>();
            _controller = new SeatController(_seatServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetSeatById_SeatNotFound_ReturnsNotFound()
        {
            // Arrange
            int seatId = 1;
            _seatServiceMock.Setup(s => s.GetSeatById(seatId)).ReturnsAsync((SeatDTO)null);

            // Act
            var result = await _controller.GetSeatById(seatId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Seat with ID {seatId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task GetAllSeats_ReturnsOk()
        {
            // Arrange
            var seats = new List<SeatDTO>
            {
                new SeatDTO { SeatId = 1, SeatNumber = "A1", IsAvailable = true },
                new SeatDTO { SeatId = 2, SeatNumber = "A2", IsAvailable = false }
            };
            _seatServiceMock.Setup(s => s.GetAllSeats()).ReturnsAsync(seats);

            // Act
            var result = await _controller.GetAllSeats();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedSeats = okResult.Value as List<SeatDTO>;
            Assert.AreEqual(2, returnedSeats.Count);
        }

        [Test]
        public async Task GetSeatsByBusId_BusNotFound_ReturnsNotFound()
        {
            // Arrange
            int busId = 1;
            _seatServiceMock.Setup(s => s.GetSeatsByBusId(busId)).ReturnsAsync((List<SeatDTO>)null);

            // Act
            var result = await _controller.GetSeatsByBusId(busId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"No seats found for bus ID {busId}.", notFoundResult.Value);
        }

        [Test]
        public async Task CreateSeatsForBus_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var createSeatsDTO = new CreateSeatsDTO
            {
                BusId = 1,
                SeatNumbers = new List<string> { "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "A10" }
            };

            var seats = new List<SeatDTO>
            {
                new SeatDTO { SeatId = 1, SeatNumber = "A1", BusId = createSeatsDTO.BusId, IsAvailable = true },
                new SeatDTO { SeatId = 2, SeatNumber = "A2", BusId = createSeatsDTO.BusId, IsAvailable = true },
                new SeatDTO { SeatId = 3, SeatNumber = "A3", BusId = createSeatsDTO.BusId, IsAvailable = true },
                new SeatDTO { SeatId = 4, SeatNumber = "A4", BusId = createSeatsDTO.BusId, IsAvailable = true },
                new SeatDTO { SeatId = 5, SeatNumber = "A5", BusId = createSeatsDTO.BusId, IsAvailable = true },
                new SeatDTO { SeatId = 6, SeatNumber = "A6", BusId = createSeatsDTO.BusId, IsAvailable = true },
                new SeatDTO { SeatId = 7, SeatNumber = "A7", BusId = createSeatsDTO.BusId, IsAvailable = true },
                new SeatDTO { SeatId = 8, SeatNumber = "A8", BusId = createSeatsDTO.BusId, IsAvailable = true },
                new SeatDTO { SeatId = 9, SeatNumber = "A9", BusId = createSeatsDTO.BusId, IsAvailable = true },
                new SeatDTO { SeatId = 10, SeatNumber = "A10", BusId = createSeatsDTO.BusId, IsAvailable = true }
            };

            _seatServiceMock.Setup(s => s.CreateSeatsForBus(createSeatsDTO)).ReturnsAsync(seats);

            // Act
            var result = await _controller.CreateSeatsForBus(createSeatsDTO);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode); 

            var response = createdResult.Value as List<SeatDTO>;
            Assert.IsNotNull(response);
            Assert.AreEqual(seats.Count, response.Count); 
        }


        [Test]
        public async Task UpdateSeatBySeatNumberAndBusId_SeatNotFound_ReturnsNotFound()
        {
            // Arrange
            string seatNumber = "A1";
            int busId = 1;
            var updateSeatDTO = new UpdateSeatDTO { IsAvailable = false };
            _seatServiceMock.Setup(s => s.UpdateSeatBySeatNumberAndBusId(seatNumber, busId, updateSeatDTO)).ReturnsAsync((SeatDTO)null);

            // Act
            var result = await _controller.UpdateSeatBySeatNumberAndBusId(seatNumber, busId, updateSeatDTO);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Seat with Seat Number {seatNumber} for Bus ID {busId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteSeatBySeatNumberAndBusId_SeatNotFound_ReturnsNotFound()
        {
            // Arrange
            string seatNumber = "A1";
            int busId = 1;
            _seatServiceMock.Setup(s => s.DeleteSeatBySeatNumberAndBusId(seatNumber, busId)).ReturnsAsync((SeatDTO)null);

            // Act
            var result = await _controller.DeleteSeatBySeatNumberAndBusId(seatNumber, busId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Seat with Seat Number {seatNumber} for Bus ID {busId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteSeatBySeatNumberAndBusId_Success_ReturnsOk()
        {
            // Arrange
            string seatNumber = "A1";
            int busId = 1;
            var seat = new SeatDTO { SeatId = 1, SeatNumber = seatNumber, BusId = busId, IsAvailable = false };
            _seatServiceMock.Setup(s => s.DeleteSeatBySeatNumberAndBusId(seatNumber, busId)).ReturnsAsync(seat);

            // Act
            var result = await _controller.DeleteSeatBySeatNumberAndBusId(seatNumber, busId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual($"Seat with Seat Number {seatNumber} for Bus ID {busId} deleted successfully.", okResult.Value);
        }
    }
}
