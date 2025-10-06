using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("OrderStatusHistory")]
    public class OrderStatusHistory
    {
        [Key]
        public int HistoryID { get; set; }

        [Required]
        public int OrderID { get; set; }

        public int? OldStatusID { get; set; }

        [Required]
        public int NewStatusID { get; set; }

        [Required]
        public int ChangedBy { get; set; }

        public DateTime ChangedDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("OldStatusID")]
        public virtual OrderStatus? OldStatus { get; set; }

        [ForeignKey("NewStatusID")]
        public virtual OrderStatus NewStatus { get; set; } = null!;

        [ForeignKey("ChangedBy")]
        public virtual User ChangedByUser { get; set; } = null!;
    }
}
