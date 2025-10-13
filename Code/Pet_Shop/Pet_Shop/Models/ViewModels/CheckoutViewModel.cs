using System.ComponentModel.DataAnnotations;

namespace Pet_Shop.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public int? AddressId { get; set; }
        
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;
        
        [Display(Name = "Phường/Xã")]
        public string Ward { get; set; } = string.Empty;
        
        [Display(Name = "Quận/Huyện")]
        public string District { get; set; } = string.Empty;
        
        [Display(Name = "Tỉnh/Thành phố")]
        public string City { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public int PaymentMethodId { get; set; }
        
        [Display(Name = "Mã giảm giá")]
        public string? PromoCode { get; set; }
        
        [Display(Name = "Ghi chú đơn hàng")]
        public string? Notes { get; set; }
        
        [Required(ErrorMessage = "Bạn phải đồng ý với điều khoản sử dụng")]
        [Display(Name = "Đồng ý với điều khoản")]
        public bool AgreeToTerms { get; set; }
    }
}
