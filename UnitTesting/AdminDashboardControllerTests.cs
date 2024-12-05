using log4net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NextStopEndPoints.Controllers;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTesting
{
    [TestFixture]
    public class AdminDashboardControllerTests
    {
        private Mock<IAdminDashboardService> _adminDashboardServiceMock;
        private Mock<ILog> _loggerMock;
        private AdminDashboardController _controller;

        [SetUp]
        public void SetUp()
        {
            _adminDashboardServiceMock = new Mock<IAdminDashboardService>();
            _loggerMock = new Mock<ILog>();
            _controller = new AdminDashboardController(_adminDashboardServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task AssignRole_Success_ReturnsOk()
        {
            // Arrange
            var assignRoleDto = new AssignRoleDTO
            {
                UserId = 1,
                Role = "operator"
            };

            _adminDashboardServiceMock.Setup(s => s.AssignRole(assignRoleDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.AssignRole(assignRoleDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Role assigned successfully.", okResult.Value);
        }

        [Test]
        public async Task AssignRole_Failure_ReturnsBadRequest()
        {
            // Arrange
            var assignRoleDto = new AssignRoleDTO
            {
                UserId = 1,
                Role = "operator"
            };

            _adminDashboardServiceMock.Setup(s => s.AssignRole(assignRoleDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.AssignRole(assignRoleDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Failed to assign role.", badRequestResult.Value);
        }

        [Test]
        public async Task GenerateReports_Success_ReturnsReport()
        {
            // Arrange
            var generateReportsDto = new GenerateReportsDTO
            {
                StartDate = System.DateTime.Now.AddDays(-30),
                EndDate = System.DateTime.Now,
                Route = "A-B",
                Operator = "Operator1"
            };

            var report = new ReportDTO
            {
                TotalBookings = 10,
                TotalRevenue = 5000m,
                Route = "A-B",
                Operator = "Operator1",
                BookingDetails = new List<BookingDetailDTO>
                {
                    new BookingDetailDTO
                    {
                        BookingId = 1,
                        UserId = 1,
                        ScheduleId = 1,
                        ReservedSeats = new List<string> { "A1", "A2" },
                        TotalFare = 1000m,
                        Status = "confirmed",
                        BookingDate = System.DateTime.Now
                    }
                }
            };

            _adminDashboardServiceMock.Setup(s => s.GenerateReports(generateReportsDto)).ReturnsAsync(report);

            // Act
            var result = await _controller.GenerateReports(generateReportsDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ReportDTO;
            Assert.IsNotNull(response);
            Assert.AreEqual(report.TotalBookings, response.TotalBookings);
            Assert.AreEqual(report.TotalRevenue, response.TotalRevenue);
            Assert.AreEqual(report.Route, response.Route);
            Assert.AreEqual(report.Operator, response.Operator);
            Assert.AreEqual(report.BookingDetails.Count, response.BookingDetails.Count);
        }

        [Test]
        public async Task GenerateReports_NoData_ReturnsNotFound()
        {
            // Arrange
            var generateReportsDto = new GenerateReportsDTO
            {
                StartDate = System.DateTime.Now.AddDays(-30),
                EndDate = System.DateTime.Now,
                Route = "A-B",
                Operator = "Operator1"
            };

            _adminDashboardServiceMock.Setup(s => s.GenerateReports(generateReportsDto)).ReturnsAsync((ReportDTO)null);

            // Act
            var result = await _controller.GenerateReports(generateReportsDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("No data available for the specified criteria.", notFoundResult.Value);
        }
    }
}
