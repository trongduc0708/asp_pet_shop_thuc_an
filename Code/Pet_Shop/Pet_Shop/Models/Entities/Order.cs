using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty; // Mã đơn hàng

        [Required]
        public int UserID { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public int StatusID { get; set; }

        [Required]
        public int PaymentMethodID { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal SubTotal { get; set; } // Tổng tiền hàng

        [Column(TypeName = "decimal(15,2)")]
        public decimal ShippingFee { get; set; } = 0; // Phí vận chuyển

        [Column(TypeName = "decimal(15,2)")]
        public decimal DiscountAmount { get; set; } = 0; // Tiền giảm giá

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalAmount { get; set; } // Tổng tiền thanh toán

        public int? PromotionID { get; set; } // Mã khuyến mãi đã sử dụng

        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; } // Ghi chú của khách hàng

        [StringLength(500)]
        public string? AdminNotes { get; set; } // Ghi chú của admin

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("StatusID")]
        public virtual OrderStatus Status { get; set; } = null!;

        [ForeignKey("PaymentMethodID")]
        public virtual PaymentMethod PaymentMethod { get; set; } = null!;

        [ForeignKey("PromotionID")]
        public virtual Promotion? Promotion { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    }
}
