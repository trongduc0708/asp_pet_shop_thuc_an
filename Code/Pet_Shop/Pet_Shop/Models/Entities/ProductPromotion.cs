using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("ProductPromotions")]
    public class ProductPromotion
    {
        [Key]
        public int ProductPromotionID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int PromotionID { get; set; }

        // Navigation properties
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; } = null!;

        [ForeignKey("PromotionID")]
        public virtual Promotion Promotion { get; set; } = null!;
    }
}
