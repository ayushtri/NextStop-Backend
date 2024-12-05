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
    public class RouteControllerTests
    {
        private Mock<IRouteService> _routeServiceMock;
        private Mock<ILog> _loggerMock;
        private RouteController _controller;

        [SetUp]
        public void SetUp()
        {
            _routeServiceMock = new Mock<IRouteService>();
            _loggerMock = new Mock<ILog>();
            _controller = new RouteController(_routeServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetRouteById_RouteNotFound_ReturnsNotFound()
        {
            // Arrange
            int routeId = 1;
            _routeServiceMock.Setup(r => r.GetRouteById(routeId)).ReturnsAsync((RouteDTO)null);

            // Act
            var result = await _controller.GetRouteById(routeId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Route with ID {routeId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task GetAllRoutes_ReturnsOk()
        {
            // Arrange
            var routes = new List<RouteDTO>
            {
                new RouteDTO { RouteId = 1, Origin = "City A", Destination = "City B" },
                new RouteDTO { RouteId = 2, Origin = "City C", Destination = "City D" }
            };
            _routeServiceMock.Setup(r => r.GetAllRoutes()).ReturnsAsync(routes);

            // Act
            var result = await _controller.GetAllRoutes();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedRoutes = okResult.Value as List<RouteDTO>;
            Assert.AreEqual(2, returnedRoutes.Count);
        }

        [Test]
        public async Task CreateRoute_Success_ReturnsCreatedAtAction()
        {
            // Arrange
            var createRouteDTO = new CreateRouteDTO
            {
                Origin = "City A",
                Destination = "City B"
            };

            var createdRoute = new RouteDTO
            {
                RouteId = 1,
                Origin = createRouteDTO.Origin,
                Destination = createRouteDTO.Destination
            };

            _routeServiceMock.Setup(r => r.CreateRoute(createRouteDTO)).ReturnsAsync(createdRoute);

            // Act
            var result = await _controller.CreateRoute(createRouteDTO);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            var route = createdResult.Value as RouteDTO;
            Assert.AreEqual(createdRoute.RouteId, route.RouteId);
            Assert.AreEqual(createdRoute.Origin, route.Origin);
            Assert.AreEqual(createdRoute.Destination, route.Destination);
        }

        [Test]
        public async Task UpdateRoute_RouteNotFound_ReturnsNotFound()
        {
            // Arrange
            int routeId = 1;
            var updateRouteDTO = new UpdateRouteDTO
            {
                Origin = "City X",
                Destination = "City Y"
            };
            _routeServiceMock.Setup(r => r.UpdateRoute(routeId, updateRouteDTO)).ReturnsAsync((RouteDTO)null);

            // Act
            var result = await _controller.UpdateRoute(routeId, updateRouteDTO);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Route with ID {routeId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteRoute_RouteNotFound_ReturnsNotFound()
        {
            // Arrange
            int routeId = 1;
            _routeServiceMock.Setup(r => r.DeleteRoute(routeId)).ReturnsAsync((RouteDTO)null);

            // Act
            var result = await _controller.DeleteRoute(routeId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Route with ID {routeId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteRoute_Success_ReturnsOk()
        {
            // Arrange
            int routeId = 1;
            var route = new RouteDTO
            {
                RouteId = routeId,
                Origin = "City A",
                Destination = "City B"
            };
            _routeServiceMock.Setup(r => r.DeleteRoute(routeId)).ReturnsAsync(route);

            // Act
            var result = await _controller.DeleteRoute(routeId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual($"Route with ID {routeId} deleted successfully.", okResult.Value);
        }
    }
}
