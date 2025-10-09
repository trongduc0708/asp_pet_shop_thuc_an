using System.ComponentModel.DataAnnotations;

namespace Pet_Shop.Models.Profile
{
    public class ProfileViewModel
    {
        public int UserID { get; set; }
        
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;
        
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }
        
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; }
        
        [Display(Name = "Lần đăng nhập cuối")]
        public DateTime? LastLoginDate { get; set; }
        
        [Display(Name = "Vai trò")]
        public string RoleName { get; set; } = string.Empty;
        
        // Customer Profile Info
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }
        
        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }
        
        [Display(Name = "Tổng đơn hàng")]
        public int TotalOrders { get; set; }
        
        [Display(Name = "Tổng chi tiêu")]
        public decimal TotalSpent { get; set; }
        
        [Display(Name = "Hạng thành viên")]
        public string MembershipLevel { get; set; } = string.Empty;
        
        [Display(Name = "Điểm tích lũy")]
        public int Points { get; set; }
    }
}
