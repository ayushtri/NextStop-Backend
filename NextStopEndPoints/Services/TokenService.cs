using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NextStopEndPoints.Data;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NextStopEndPoints.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        NextStopDbContext _context;

        public TokenService(IConfiguration configuration, NextStopDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string GenerateToken(TokenDTO tokenDTO)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, tokenDTO.Email),
                new Claim(ClaimTypes.NameIdentifier, tokenDTO.UserId.ToString()),
                new Claim(ClaimTypes.Role, tokenDTO.Role) 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task SaveRefreshToken(string email, string token)
        {
            var refreshToken = new RefreshToken
            {
                Email = email,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshToken);

            await _context.SaveChangesAsync();
        }

        public async Task<string> RetrieveEmailByRefreshToken(string refreshToken)
        {
            var tokenRecord = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ExpiryDate > DateTime.UtcNow);

            return tokenRecord?.Email;
        }

        public async Task<bool> RevokeRefreshToken(string refreshToken)
        {
            var tokenRecord = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (tokenRecord != null)
            {
                _context.RefreshTokens.Remove(tokenRecord);

                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
