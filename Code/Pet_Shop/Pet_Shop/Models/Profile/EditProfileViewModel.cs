using System.ComponentModel.DataAnnotations;

namespace Pet_Shop.Models.Profile
{
    public class EditProfileViewModel
    {
        public int UserID { get; set; }
        
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Địa chỉ không được quá 200 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        
        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }
        
        // Read-only fields
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Vai trò")]
        public string RoleName { get; set; } = string.Empty;
    }
}
