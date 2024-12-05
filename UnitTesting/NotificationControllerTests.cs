using log4net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NextStopEndPoints.Controllers;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Models;
using NextStopEndPoints.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTesting
{
    [TestFixture]
    public class NotificationControllerTests
    {
        private Mock<INotificationService> _notificationServiceMock;
        private Mock<ILog> _loggerMock;
        private NotificationController _controller;

        [SetUp]
        public void SetUp()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILog>();
            _controller = new NotificationController(_notificationServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task SendNotification_Success_ReturnsOk()
        {
            // Arrange
            var sendNotificationDto = new SendNotificationDTO
            {
                UserId = 1,
                Message = "Test Notification",
                NotificationType = NotificationTypeEnum.Email // Use enum directly
            };

            _notificationServiceMock.Setup(n => n.SendNotification(sendNotificationDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.SendNotification(sendNotificationDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Notification sent successfully.", okResult.Value);
        }

        [Test]
        public async Task SendNotification_Failure_ReturnsBadRequest()
        {
            // Arrange
            var sendNotificationDto = new SendNotificationDTO
            {
                UserId = 1,
                Message = "Test Notification",
                NotificationType = NotificationTypeEnum.Email // Use enum directly
            };

            _notificationServiceMock.Setup(n => n.SendNotification(sendNotificationDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.SendNotification(sendNotificationDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Failed to send notification.", badRequestResult.Value);
        }

        [Test]
        public async Task ViewNotifications_Found_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            var notifications = new List<NotificationDTO>
            {
                new NotificationDTO { NotificationId = 1, Message = "Notification 1", SentDate = System.DateTime.Now, NotificationType = NotificationTypeEnum.Email },
                new NotificationDTO { NotificationId = 2, Message = "Notification 2", SentDate = System.DateTime.Now, NotificationType = NotificationTypeEnum.SMS }
            };

            _notificationServiceMock.Setup(n => n.ViewNotifications(userId)).ReturnsAsync(notifications);

            // Act
            var result = await _controller.ViewNotifications(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as List<NotificationDTO>;
            Assert.IsNotNull(response);
            Assert.AreEqual(notifications.Count, response.Count);
        }

        [Test]
        public async Task ViewNotifications_NotFound_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;
            _notificationServiceMock.Setup(n => n.ViewNotifications(userId)).ReturnsAsync(new List<NotificationDTO>());

            // Act
            var result = await _controller.ViewNotifications(userId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual("No notifications found for this user.", notFoundResult.Value);
        }
    }
}
