using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("Inventory")]
    public class Inventory
    {
        [Key]
        public int InventoryID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int QuantityInStock { get; set; } = 0;

        public int MinStockLevel { get; set; } = 10; // Mức tồn kho tối thiểu

        public int MaxStockLevel { get; set; } = 1000; // Mức tồn kho tối đa

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; } = null!;
    }
}
