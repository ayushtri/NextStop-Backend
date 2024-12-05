using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [StringLength(10)]
        [Phone]
        public string Phone { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression("^(passenger|operator|admin)$", ErrorMessage = "Role must be 'passenger', 'operator', or 'admin'.")]
        public string Role { get; set; }

        [Required]
        public bool IsActive { get; set; } = true; // Default to active

        // Navigation Properties
        public virtual ICollection<Bus> Buses { get; set; } // For operators
        public virtual ICollection<AdminAction> AdminActions { get; set; } // For admins
        public virtual ICollection<Booking> Bookings { get; set; } // For passengers
        public virtual ICollection<Notification> Notifications { get; set; } // For all users
    }
}