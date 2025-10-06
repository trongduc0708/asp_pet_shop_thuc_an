using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("ProductImages")]
    public class ProductImage
    {
        [Key]
        public int ImageID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageURL { get; set; } = string.Empty;

        [StringLength(100)]
        public string? AltText { get; set; }

        public bool IsPrimary { get; set; } = false; // Hình ảnh chính

        public int SortOrder { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; } = null!;
    }
}
