using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.DTOs
{
    public class LogoutDTO
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
