using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Mã sản phẩm không được vượt quá 50 ký tự")]
        [Display(Name = "Mã sản phẩm")]
        public string? ProductCode { get; set; } // Mã sản phẩm

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [Display(Name = "Danh mục")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Thương hiệu là bắt buộc")]
        [Display(Name = "Thương hiệu")]
        public int BrandID { get; set; }

        [Required(ErrorMessage = "Loại sản phẩm là bắt buộc")]
        [StringLength(20, ErrorMessage = "Loại sản phẩm không được vượt quá 20 ký tự")]
        [Display(Name = "Loại sản phẩm")]
        public string ProductType { get; set; } = string.Empty; // 'Food', 'Accessory'

        [Required(ErrorMessage = "Loại thú cưng là bắt buộc")]
        [StringLength(20, ErrorMessage = "Loại thú cưng không được vượt quá 20 ký tự")]
        [Display(Name = "Loại thú cưng")]
        public string PetType { get; set; } = string.Empty; // 'Dog', 'Cat', 'Both'

        [Range(0, 999.99, ErrorMessage = "Trọng lượng phải từ 0 đến 999.99 kg")]
        [Column(TypeName = "decimal(8,2)")]
        [Display(Name = "Trọng lượng (kg)")]
        public decimal? Weight { get; set; } // Trọng lượng (kg)

        [StringLength(100, ErrorMessage = "Kích thước không được vượt quá 100 ký tự")]
        [Display(Name = "Kích thước")]
        public string? Dimensions { get; set; } // Kích thước

        [Display(Name = "Hạn sử dụng")]
        public DateTime? ExpiryDate { get; set; } // Hạn sử dụng

        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự")]
        [Display(Name = "Mô tả chi tiết")]
        public string? Description { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả ngắn không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả ngắn")]
        public string? ShortDescription { get; set; }

        [Required(ErrorMessage = "Giá bán là bắt buộc")]
        [Range(0, 999999999.99, ErrorMessage = "Giá bán phải từ 0 đến 999,999,999.99")]
        [Column(TypeName = "decimal(15,2)")]
        [Display(Name = "Giá bán")]
        public decimal Price { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Giá khuyến mãi phải từ 0 đến 999,999,999.99")]
        [SalePriceValidation]
        [Column(TypeName = "decimal(15,2)")]
        [Display(Name = "Giá khuyến mãi")]
        public decimal? SalePrice { get; set; } // Giá khuyến mãi

        [Range(0, 999999999.99, ErrorMessage = "Giá nhập phải từ 0 đến 999,999,999.99")]
        [Column(TypeName = "decimal(15,2)")]
        [Display(Name = "Giá nhập")]
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

    // Custom validation attribute for sale price
    public class SalePriceValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is Product product)
            {
                if (product.SalePrice.HasValue && product.SalePrice > product.Price)
                {
                    return new ValidationResult("Giá khuyến mãi không được lớn hơn giá bán");
                }
            }
            
            return ValidationResult.Success;
        }
    }
}
