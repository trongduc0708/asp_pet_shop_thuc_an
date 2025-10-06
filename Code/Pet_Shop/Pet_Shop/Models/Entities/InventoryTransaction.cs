using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("InventoryTransactions")]
    public class InventoryTransaction
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        [StringLength(20)]
        public string TransactionType { get; set; } = string.Empty; // 'Import', 'Export', 'Adjustment'

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? UnitPrice { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? TotalValue { get; set; }

        [StringLength(50)]
        public string? ReferenceNumber { get; set; } // Số phiếu nhập/xuất

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; } = null!;

        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; } = null!;
    }
}
