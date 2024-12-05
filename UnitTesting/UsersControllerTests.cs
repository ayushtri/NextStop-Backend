using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using NextStopEndPoints.Controllers;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Services;
using NextStopEndPoints.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace UnitTesting
{
    [TestFixture]
    public class UsersControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<ILog> _loggerMock;
        private UsersController _controller;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILog>();
            _controller = new UsersController(_userServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetCurrentUser_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;

            // Mock the user claim to simulate an authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            _userServiceMock.Setup(u => u.GetUserById(userId)).ReturnsAsync((UserDTO)null);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("User not found.", notFoundResult.Value);
        }


        [Test]
        public async Task GetAllUsers_NoUsersFound_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(u => u.GetAllUsers()).ReturnsAsync(new List<UserDTO>());

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("No users found.", notFoundResult.Value);
        }

        [Test]
        public async Task GetUserById_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            _userServiceMock.Setup(u => u.GetUserById(userId)).ReturnsAsync((UserDTO)null);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"User with ID {userId} not found.", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteUser_UserNotFound_ReturnsNotFound()
        {
            int userId = 1;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            _userServiceMock.Setup(u => u.GetUserById(userId)).ReturnsAsync((UserDTO)null);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult); 
            Assert.AreEqual(404, notFoundResult.StatusCode); 
            Assert.AreEqual($"User with ID {userId} not found.", notFoundResult.Value); 
        }

        [Test]
        public async Task UpdateUser_Success_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            int updatedUserId = userId; 
            string updatedName = "John Doe";
            var updateUserDTO = new UpdateUserDTO { Name = updatedName };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var existingUser = new UserDTO { UserId = userId, Name = "Old Name" };
            _userServiceMock.Setup(u => u.GetUserById(userId)).ReturnsAsync(existingUser);
            _userServiceMock.Setup(u => u.UpdateUser(userId, updateUserDTO)).ReturnsAsync(new UserDTO { UserId = updatedUserId, Name = updatedName });

            // Act
            var result = await _controller.UpdateUser(userId, updateUserDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

        }

        [Test]
        public async Task ResetEmail_Success_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            string newEmail = "newemail@example.com";
            var existingUser = new UserDTO { UserId = userId, Email = "oldemail@example.com" };

            _userServiceMock.Setup(u => u.GetUserById(userId)).ReturnsAsync(existingUser);
            _userServiceMock.Setup(u => u.IsEmailUnique(newEmail)).ReturnsAsync(true); 
            _userServiceMock.Setup(u => u.ResetEmail(userId, newEmail)).Returns(Task.CompletedTask); 

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _controller.ResetEmail(userId, newEmail);

            // Assert
            Assert.IsNotNull(result); 
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult); 
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Email reset successfully.", okResult.Value);
        }

        [Test]
        public async Task ReactivateUser_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;
            _userServiceMock.Setup(u => u.GetUserById(userId)).ReturnsAsync((UserDTO)null);

            // Act
            var result = await _controller.ReactivateUser(userId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"User with ID {userId} not found.", notFoundResult.Value);
        }
    }
}
