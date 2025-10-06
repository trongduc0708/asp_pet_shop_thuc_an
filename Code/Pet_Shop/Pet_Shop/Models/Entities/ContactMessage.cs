using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("ContactMessages")]
    public class ContactMessage
    {
        [Key]
        public int MessageID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "New"; // 'New', 'Read', 'Replied', 'Closed'

        [StringLength(2000)]
        public string? ReplyMessage { get; set; }

        public int? RepliedBy { get; set; }

        public DateTime? RepliedDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("RepliedBy")]
        public virtual User? RepliedByUser { get; set; }
    }
}
