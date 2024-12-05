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
    public class ScheduleControllerTests
    {
        private Mock<IScheduleService> _scheduleServiceMock;
        private Mock<ILog> _loggerMock;
        private ScheduleController _controller;

        [SetUp]
        public void SetUp()
        {
            _scheduleServiceMock = new Mock<IScheduleService>();
            _loggerMock = new Mock<ILog>();
            _controller = new ScheduleController(_scheduleServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetScheduleById_ScheduleNotFound_ReturnsNotFound()
        {
            // Arrange
            int scheduleId = 1;
            _scheduleServiceMock.Setup(s => s.GetScheduleById(scheduleId)).ReturnsAsync((ScheduleDTO)null);

            // Act
            var result = await _controller.GetScheduleById(scheduleId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Schedule with ID {scheduleId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task GetAllSchedules_ReturnsOk()
        {
            // Arrange
            var schedules = new List<ScheduleDTO>
            {
                new ScheduleDTO { ScheduleId = 1, DepartureTime = DateTime.Now, ArrivalTime = DateTime.Now.AddHours(2) },
                new ScheduleDTO { ScheduleId = 2, DepartureTime = DateTime.Now.AddHours(3), ArrivalTime = DateTime.Now.AddHours(5) }
            };
            _scheduleServiceMock.Setup(s => s.GetAllSchedules()).ReturnsAsync(schedules);

            // Act
            var result = await _controller.GetAllSchedules();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedSchedules = okResult.Value as List<ScheduleDTO>;
            Assert.AreEqual(2, returnedSchedules.Count);
        }

        [Test]
        public async Task GetSchedulesByRouteId_RouteNotFound_ReturnsNotFound()
        {
            // Arrange
            int routeId = 1;
            _scheduleServiceMock.Setup(s => s.GetSchedulesByRouteId(routeId)).ReturnsAsync((List<ScheduleDTO>)null);

            // Act
            var result = await _controller.GetSchedulesByRouteId(routeId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"No schedules found for route ID {routeId}.", notFoundResult.Value);
        }

        [Test]
        public async Task CreateSchedule_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var createScheduleDTO = new CreateScheduleDTO
            {
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2),
                RouteId = 1,
                BusId = 1
            };

            var createdSchedule = new ScheduleDTO
            {
                ScheduleId = 1,
                DepartureTime = createScheduleDTO.DepartureTime,
                ArrivalTime = createScheduleDTO.ArrivalTime,
                RouteId = createScheduleDTO.RouteId,
                BusId = createScheduleDTO.BusId
            };

            _scheduleServiceMock.Setup(s => s.CreateSchedule(createScheduleDTO)).ReturnsAsync(createdSchedule);

            // Act
            var result = await _controller.CreateSchedule(createScheduleDTO);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            var schedule = createdResult.Value as ScheduleDTO;
            Assert.AreEqual(createdSchedule.ScheduleId, schedule.ScheduleId);
            Assert.AreEqual(createdSchedule.DepartureTime, schedule.DepartureTime);
            Assert.AreEqual(createdSchedule.ArrivalTime, schedule.ArrivalTime);
        }

        [Test]
        public async Task UpdateSchedule_ScheduleNotFound_ReturnsNotFound()
        {
            // Arrange
            int scheduleId = 1;
            var updateScheduleDTO = new UpdateScheduleDTO
            {
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2)
            };
            _scheduleServiceMock.Setup(s => s.UpdateSchedule(scheduleId, updateScheduleDTO)).ReturnsAsync((ScheduleDTO)null);

            // Act
            var result = await _controller.UpdateSchedule(scheduleId, updateScheduleDTO);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Schedule with ID {scheduleId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteSchedule_ScheduleNotFound_ReturnsNotFound()
        {
            // Arrange
            int scheduleId = 1;
            _scheduleServiceMock.Setup(s => s.DeleteSchedule(scheduleId)).ReturnsAsync((ScheduleDTO)null);

            // Act
            var result = await _controller.DeleteSchedule(scheduleId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Schedule with ID {scheduleId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteSchedule_Success_ReturnsOk()
        {
            // Arrange
            int scheduleId = 1;
            var schedule = new ScheduleDTO
            {
                ScheduleId = scheduleId,
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2),
                RouteId = 1,
                BusId = 1
            };
            _scheduleServiceMock.Setup(s => s.DeleteSchedule(scheduleId)).ReturnsAsync(schedule);

            // Act
            var result = await _controller.DeleteSchedule(scheduleId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual($"Schedule with ID {scheduleId} deleted successfully.", okResult.Value);
        }
    }
}
