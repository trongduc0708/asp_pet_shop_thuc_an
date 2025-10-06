using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("Brands")]
    public class Brand
    {
        [Key]
        public int BrandID { get; set; }

        [Required]
        [StringLength(100)]
        public string BrandName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? LogoURL { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
