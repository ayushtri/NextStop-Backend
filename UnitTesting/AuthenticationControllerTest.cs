using log4net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NextStopEndPoints.Controllers;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Services;

namespace UnitTesting
{
    [TestFixture]
    public class AuthenticationControllerTest
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<ILog> _loggerMock;
        private AuthenticationController _controller;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<ILog>();
            _controller = new AuthenticationController(_userServiceMock.Object, _tokenServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Register_UserAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var userDTO = new CreateUserDTO
            {
                Email = "test@example.com",
                Password = "Password123",
                Name = "John Doe",
                Phone = "1234567890",
                Address = "123 Street",
                Role = "user"
            };

            _userServiceMock.Setup(u => u.IsEmailUnique(userDTO.Email)).ReturnsAsync(false);

            // Act
            var result = await _controller.Register(userDTO);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Email is already in use.", badRequestResult.Value);
        }

        [Test]
        public async Task Register_Success_ReturnsOkWithUserData()
        {
            // Arrange
            var userDTO = new CreateUserDTO
            {
                Email = "test@example.com",
                Password = "Password123",
                Name = "John Doe",
                Phone = "1234567890",
                Address = "123 Street",
                Role = "user"
            };

            var createdUser = new UserDTO
            {
                UserId = 1,
                Email = userDTO.Email,
                Name = userDTO.Name,
                Phone = userDTO.Phone,
                Address = userDTO.Address,
                Role = userDTO.Role,
                IsActive = true
            };

            _userServiceMock.Setup(u => u.IsEmailUnique(userDTO.Email)).ReturnsAsync(true);
            _userServiceMock.Setup(u => u.CreateUser(userDTO)).ReturnsAsync(createdUser);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<TokenDTO>())).Returns("jwt_token");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("refresh_token");
            _tokenServiceMock.Setup(t => t.SaveRefreshToken(userDTO.Email, "refresh_token")).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Register(userDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as RegisterResponseDTO;
            Assert.IsNotNull(response);

            // Check user details
            Assert.AreEqual(createdUser.Email, response.User.Email);
            Assert.AreEqual(createdUser.Name, response.User.Name);
            Assert.AreEqual(createdUser.Phone, response.User.Phone);
            Assert.AreEqual(createdUser.Address, response.User.Address);
            Assert.AreEqual(createdUser.Role, response.User.Role);
            Assert.AreEqual(createdUser.IsActive, response.User.IsActive);

            // Check token details
            Assert.AreEqual("jwt_token", response.JwtToken);
            Assert.AreEqual("refresh_token", response.RefreshToken);
        }


        [Test]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Email = "test@example.com",
                Password = "InvalidPassword"
            };

            _userServiceMock.Setup(u => u.GetUserByEmailAndPassword(loginDTO.Email, loginDTO.Password)).ReturnsAsync((UserDTO)null);

            // Act
            var result = await _controller.Login(loginDTO);

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
            Assert.AreEqual("Invalid email or password.", unauthorizedResult.Value);
        }

        [Test]
        public async Task Login_Success_ReturnsJwtToken()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            var userDTO = new UserDTO
            {
                UserId = 1,
                Email = "test@example.com",
                Role = "user"
            };

            _userServiceMock.Setup(u => u.GetUserByEmailAndPassword(loginDTO.Email, loginDTO.Password)).ReturnsAsync(userDTO);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<TokenDTO>())).Returns("jwt_token");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("refresh_token");
            _tokenServiceMock.Setup(t => t.SaveRefreshToken(userDTO.Email, "refresh_token")).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Login(loginDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            // Extracting the response as LoginResponseDTO
            var response = okResult.Value as LoginResponseDTO;
            Assert.IsNotNull(response);

            // Check if the response contains the expected properties
            Assert.AreEqual("jwt_token", response.JwtToken);
            Assert.AreEqual("refresh_token", response.RefreshToken);
        }





        [Test]
        public async Task RenewTokens_InvalidRefreshToken_ReturnsUnauthorized()
        {
            // Arrange
            var invalidRefreshToken = "invalid_refresh_token";
            _tokenServiceMock.Setup(t => t.RetrieveEmailByRefreshToken(invalidRefreshToken)).ReturnsAsync((string)null);

            // Act
            var result = await _controller.RenewTokens(invalidRefreshToken);

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
            Assert.AreEqual("Invalid or expired refresh token.", unauthorizedResult.Value);
        }

        [Test]
        public async Task RenewTokens_Success_ReturnsNewTokens()
        {
            // Arrange
            var refreshToken = "valid_refresh_token";
            var email = "test@example.com";
            var userDTO = new UserDTO
            {
                UserId = 1,
                Email = email,
                Role = "user"
            };

            _tokenServiceMock.Setup(t => t.RetrieveEmailByRefreshToken(refreshToken)).ReturnsAsync(email);
            _userServiceMock.Setup(u => u.GetUserByEmail(email)).ReturnsAsync(userDTO);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<TokenDTO>())).Returns("new_jwt_token");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("new_refresh_token");
            _tokenServiceMock.Setup(t => t.SaveRefreshToken(email, "new_refresh_token")).Returns(Task.CompletedTask);

            // Change the setup to return Task.FromResult(true) to match Task<bool>
            _tokenServiceMock.Setup(t => t.RevokeRefreshToken(refreshToken)).ReturnsAsync(true);

            // Act
            var result = await _controller.RenewTokens(refreshToken);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as RenewTokenResponseDTO;
            Assert.AreEqual("new_jwt_token", response.NewJwtToken);
            Assert.AreEqual("new_refresh_token", response.NewRefreshToken);
        }


        [Test]
        public async Task Logout_Success_ReturnsOk()
        {
            // Arrange
            var refreshToken = "valid_refresh_token";
            _tokenServiceMock.Setup(t => t.RevokeRefreshToken(refreshToken)).ReturnsAsync(true);

            // Act
            var result = await _controller.Logout(refreshToken);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Logged out successfully.", okResult.Value);
        }

        [Test]
        public async Task Logout_Failed_ReturnsBadRequest()
        {
            // Arrange
            var refreshToken = "invalid_refresh_token";
            _tokenServiceMock.Setup(t => t.RevokeRefreshToken(refreshToken)).ReturnsAsync(false);

            // Act
            var result = await _controller.Logout(refreshToken);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Failed to log out, refresh token not found.", badRequestResult.Value);
        }
    }
}