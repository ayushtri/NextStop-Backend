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
    public class BusControllerTests
    {
        private Mock<IBusService> _busServiceMock;
        private Mock<ILog> _loggerMock;
        private BusController _controller;

        [SetUp]
        public void SetUp()
        {
            _busServiceMock = new Mock<IBusService>();
            _loggerMock = new Mock<ILog>();
            _controller = new BusController(_busServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetBusById_BusNotFound_ReturnsNotFound()
        {
            // Arrange
            int busId = 1;
            _busServiceMock.Setup(b => b.GetBusById(busId)).ReturnsAsync((BusDTO)null);

            // Act
            var result = await _controller.GetBusById(busId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Bus with ID {busId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task GetAllBuses_ReturnsOk()
        {
            // Arrange
            var buses = new List<BusDTO>
            {
                new BusDTO { BusId = 1, BusNumber = "B123", OperatorName = "Operator 1" },
                new BusDTO { BusId = 2, BusNumber = "B124", OperatorName = "Operator 2" }
            };
            _busServiceMock.Setup(b => b.GetAllBuses()).ReturnsAsync(buses);

            // Act
            var result = await _controller.GetAllBuses();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedBuses = okResult.Value as List<BusDTO>;
            Assert.AreEqual(2, returnedBuses.Count);
        }

        [Test]
        public async Task GetBusesByOperatorId_BusesNotFound_ReturnsNotFound()
        {
            // Arrange
            int operatorId = 1;
            _busServiceMock.Setup(b => b.GetBusesByOperatorId(operatorId)).ReturnsAsync(new List<BusDTO>());

            // Act
            var result = await _controller.GetBusesByOperatorId(operatorId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"No buses found for operator ID {operatorId}.", notFoundResult.Value);
        }

        [Test]
        public async Task CreateBus_BusNumberNotUnique_ReturnsBadRequest()
        {
            // Arrange
            var createBusDTO = new CreateBusDTO
            {
                BusNumber = "B123",
                OperatorId = 1
            };

            _busServiceMock.Setup(b => b.BusNumberUnique(createBusDTO.BusNumber)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateBus(createBusDTO);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("The bus number is already in use.", badRequestResult.Value);
        }

        [Test]
        public async Task CreateBus_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var createBusDTO = new CreateBusDTO
            {
                BusNumber = "B123",
                OperatorId = 1
            };

            var createdBus = new BusDTO
            {
                BusId = 1,
                BusNumber = createBusDTO.BusNumber,
                OperatorName = "Operator 1"
            };

            _busServiceMock.Setup(b => b.BusNumberUnique(createBusDTO.BusNumber)).ReturnsAsync(true);
            _busServiceMock.Setup(b => b.CreateBus(createBusDTO)).ReturnsAsync(createdBus);

            // Act
            var result = await _controller.CreateBus(createBusDTO);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            var bus = createdResult.Value as BusDTO;
            Assert.AreEqual(createdBus.BusId, bus.BusId);
            Assert.AreEqual(createdBus.BusNumber, bus.BusNumber);
        }

        [Test]
        public async Task UpdateBus_BusNotFound_ReturnsNotFound()
        {
            // Arrange
            int busId = 1;
            var updateBusDTO = new UpdateBusDTO
            {
                BusNumber = "B125"
            };
            _busServiceMock.Setup(b => b.GetBusById(busId)).ReturnsAsync((BusDTO)null);

            // Act
            var result = await _controller.UpdateBus(busId, updateBusDTO);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Bus with ID {busId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteBus_BusNotFound_ReturnsNotFound()
        {
            // Arrange
            int busId = 1;
            _busServiceMock.Setup(b => b.DeleteBus(busId)).ReturnsAsync((BusDTO)null);

            // Act
            var result = await _controller.DeleteBus(busId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Bus with ID {busId} not found.", notFoundResult.Value);
        }
    }
}
