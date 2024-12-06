using log4net;
using Microsoft.AspNetCore.Mvc;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Services;
using System;
using System.Threading.Tasks;

namespace NextStopEndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ILog _logger;

        public AuthenticationController(IUserService userService, ITokenService tokenService, ILog logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO userDTO)
        {
            try
            {
                var isEmailUnique = await _userService.IsEmailUnique(userDTO.Email);
                if (!isEmailUnique)
                {
                    return BadRequest("Email is already in use.");
                }

                // Create the user
                var createdUser = await _userService.CreateUser(userDTO);

                var tokenDTO = new TokenDTO
                {
                    Email = createdUser.Email,
                    Role = createdUser.Role,
                    UserId = createdUser.UserId
                };

                // Generate JWT token
                var jwtToken = _tokenService.GenerateToken(tokenDTO); 
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Save the refresh token in the database
                await _tokenService.SaveRefreshToken(userDTO.Email, refreshToken);

                var registerResponse = new RegisterResponseDTO
                {
                    User = new UserDTO
                    {
                        UserId = createdUser.UserId,
                        Name = createdUser.Name,
                        Email = createdUser.Email,
                        Phone = createdUser.Phone,
                        Address = createdUser.Address,
                        Role = createdUser.Role,
                        IsActive = createdUser.IsActive
                    },
                    JwtToken = jwtToken,
                    RefreshToken = refreshToken
                };

                return Ok(registerResponse);
            }
            catch (Exception ex)
            {
                _logger.Error("Registration failed", ex); 
                return BadRequest($"Registration failed: {ex.Message}");
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var user = await _userService.GetUserByEmailAndPassword(loginDTO.Email, loginDTO.Password);
                if (user == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                var tokenDTO = new TokenDTO
                {
                    Email = user.Email,
                    Role = user.Role,
                    UserId = user.UserId
                };

                var jwtToken = _tokenService.GenerateToken(tokenDTO); // Generate the JWT token
                var refreshToken = _tokenService.GenerateRefreshToken(); // Generate the refresh token

                await _tokenService.SaveRefreshToken(user.Email, refreshToken); // Save the refresh token

                // Return jwtToken and refreshToken in the response
                var response = new LoginResponseDTO
                {
                    UserId = user.UserId,
                    JwtToken = jwtToken,
                    RefreshToken = refreshToken
                };

                return Ok(response); 
            }
            catch (Exception ex)
            {
                _logger.Error("Login failed", ex);
                return BadRequest($"Login failed: {ex.Message}");
            }
        }


        [HttpPost("renew-tokens")]
        public async Task<IActionResult> RenewTokens([FromBody] string refreshToken)
        {
            try
            {
                // Retrieve the email associated with the refresh token
                var email = await _tokenService.RetrieveEmailByRefreshToken(refreshToken);

                if (email == null)
                {
                    return Unauthorized("Invalid or expired refresh token.");
                }

                var user = await _userService.GetUserByEmail(email);

                
                var tokenDTO = new TokenDTO
                {
                    Email = email,
                    Role = user.Role,
                    UserId = user.UserId
                };

                // Generate new JWT token
                var newJwtToken = _tokenService.GenerateToken(tokenDTO); // Pass TokenDTO instead of CreateUserDTO

                // Generate a new refresh token
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                // Save the new refresh token and revoke the old one
                await _tokenService.RevokeRefreshToken(refreshToken);
                await _tokenService.SaveRefreshToken(email, newRefreshToken);

                var response = new RenewTokenResponseDTO
                {
                    NewJwtToken = newJwtToken,
                    NewRefreshToken = newRefreshToken
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to renew tokens", ex);
                return BadRequest($"Failed to renew tokens: {ex.Message}");
            }
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDTO logoutDto)
        {
            try
            {
                var result = await _tokenService.RevokeRefreshToken(logoutDto.RefreshToken);

                if (!result)
                {
                    return BadRequest("Failed to log out, refresh token not found.");
                }

                return Ok(new LogoutResponseDTO
                {
                    Success = true,
                    Message = "Logged out successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.Error("Logout failed", ex);
                return BadRequest($"Logout failed: {ex.Message}");
            }
        }

    }
}
