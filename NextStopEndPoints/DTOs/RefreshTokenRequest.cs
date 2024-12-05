using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.DTOs
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
