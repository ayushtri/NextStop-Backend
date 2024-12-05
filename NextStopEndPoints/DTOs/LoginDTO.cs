using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.DTOs
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
    }
}
