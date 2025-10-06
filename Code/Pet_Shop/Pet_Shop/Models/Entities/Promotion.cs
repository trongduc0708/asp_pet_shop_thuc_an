using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("Promotions")]
    public class Promotion
    {
        [Key]
        public int PromotionID { get; set; }

        [Required]
        [StringLength(50)]
        public string PromotionCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string PromotionName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(20)]
        public string DiscountType { get; set; } = string.Empty; // 'Percentage', 'FixedAmount'

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal DiscountValue { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal MinOrderAmount { get; set; } = 0; // Đơn hàng tối thiểu

        [Column(TypeName = "decimal(15,2)")]
        public decimal? MaxDiscountAmount { get; set; } // Giảm giá tối đa

        public int? UsageLimit { get; set; } // Số lần sử dụng tối đa

        public int UsedCount { get; set; } = 0; // Số lần đã sử dụng

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<ProductPromotion> ProductPromotions { get; set; } = new List<ProductPromotion>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
