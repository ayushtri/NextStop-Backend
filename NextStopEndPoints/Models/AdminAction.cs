using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStopEndPoints.Models
{
    public class AdminAction
    {
        [Key]
        public int ActionId { get; set; }

        [ForeignKey("User")]
        public int AdminId { get; set; }
        public virtual User Admin { get; set; }

        [StringLength(100)]
        public string ActionType { get; set; }

        public DateTime ActionTimestamp { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string Details { get; set; }
    }
}
