using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("Banners")]
    public class Banner
    {
        [Key]
        public int BannerID { get; set; }

        [Required]
        [StringLength(200)]
        public string BannerName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string ImageURL { get; set; } = string.Empty;

        [StringLength(255)]
        public string? LinkURL { get; set; }

        [StringLength(100)]
        public string? AltText { get; set; }

        [StringLength(50)]
        public string? Position { get; set; } // 'Homepage', 'Category', 'Product'

        public int SortOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
