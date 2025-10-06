using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("CustomerProfiles")]
    public class CustomerProfile
    {
        [Key]
        public int ProfileID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        public int TotalOrders { get; set; } = 0;

        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalSpent { get; set; } = 0;

        [StringLength(20)]
        public string MembershipLevel { get; set; } = "Bronze"; // Bronze, Silver, Gold, Platinum

        public int Points { get; set; } = 0;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;
    }
}
