using System.ComponentModel.DataAnnotations;
using Pet_Shop.Models.Entities;

namespace Pet_Shop.Models.ViewModels
{
    public class CustomerViewModel
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? Phone { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        // Customer Profile
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalSpent { get; set; }

        [StringLength(20)]
        public string MembershipLevel { get; set; } = "Bronze";

        public int Points { get; set; }

        // Additional info
        public string RoleName { get; set; } = string.Empty;
        public int AddressCount { get; set; }
        public int OrderCount { get; set; }
    }

    public class CustomerEditViewModel
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? Phone { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string? Address { get; set; }

        public bool IsActive { get; set; }

        // Customer Profile
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Hạng thành viên là bắt buộc")]
        [StringLength(20)]
        public string MembershipLevel { get; set; } = "Bronze";

        public List<string> AvailableMembershipLevels { get; set; } = new List<string>();
        public List<string> AvailableGenders { get; set; } = new List<string>();
    }

    public class CustomerDetailsViewModel
    {
        public User Customer { get; set; } = null!;
        public CustomerProfile? CustomerProfile { get; set; }
        public List<Address> Addresses { get; set; } = new List<Address>();
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public CustomerStatistics Statistics { get; set; } = new CustomerStatistics();
    }

    public class CustomerStatistics
    {
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int Points { get; set; }
        public string MembershipLevel { get; set; } = string.Empty;
        public DateTime? LastOrderDate { get; set; }
        public int DaysSinceLastOrder { get; set; }
    }

    public class CustomerSearchViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public string? MembershipLevel { get; set; }
        public List<string> AvailableMembershipLevels { get; set; } = new List<string>();
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
    }
}
