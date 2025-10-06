using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Shop.Models.Entities
{
    [Table("Addresses")]
    public class Address
    {
        [Key]
        public int AddressID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string AddressLine { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Ward { get; set; } // Phường/Xã

        [StringLength(100)]
        public string? District { get; set; } // Quận/Huyện

        [StringLength(100)]
        public string? City { get; set; } // Tỉnh/Thành phố

        public bool IsDefault { get; set; } = false; // Địa chỉ mặc định

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual User User { get; set; } = null!;
    }
}
