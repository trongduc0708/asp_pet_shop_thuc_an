using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ProductCode { get; set; } // Mã sản phẩm

        [Required]
        public int CategoryID { get; set; }

        [Required]
        public int BrandID { get; set; }

        [Required]
        [StringLength(20)]
        public string ProductType { get; set; } = string.Empty; // 'Food', 'Accessory'

        [Required]
        [StringLength(20)]
        public string PetType { get; set; } = string.Empty; // 'Dog', 'Cat', 'Both'

        [Column(TypeName = "decimal(8,2)")]
        public decimal? Weight { get; set; } // Trọng lượng (kg)

        [StringLength(100)]
        public string? Dimensions { get; set; } // Kích thước

        public DateTime? ExpiryDate { get; set; } // Hạn sử dụng

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? ShortDescription { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? SalePrice { get; set; } // Giá khuyến mãi

        [Column(TypeName = "decimal(15,2)")]
        public decimal? Cost { get; set; } // Giá nhập

        public bool IsNew { get; set; } = false; // Sản phẩm mới

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; } = false; // Sản phẩm nổi bật

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; } = null!;

        [ForeignKey("BrandID")]
        public virtual Brand Brand { get; set; } = null!;

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public virtual Inventory? Inventory { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
        public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<ProductPromotion> ProductPromotions { get; set; } = new List<ProductPromotion>();
    }
}
