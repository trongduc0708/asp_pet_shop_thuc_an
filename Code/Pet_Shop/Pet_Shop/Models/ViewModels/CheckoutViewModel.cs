using System.ComponentModel.DataAnnotations;
using System;

namespace Pet_Shop.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public int? AddressId { get; set; }
        
        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }
        
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }
        
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        
        [Display(Name = "Phường/Xã")]
        public string? Ward { get; set; }
        
        [Display(Name = "Quận/Huyện")]
        public string? District { get; set; }
        
        [Display(Name = "Tỉnh/Thành phố")]
        public string? City { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public int PaymentMethodId { get; set; }
        
        [Display(Name = "Mã giảm giá")]
        public string? PromoCode { get; set; }
        
        [Display(Name = "Ghi chú đơn hàng")]
        public string? Notes { get; set; }
        
        [Display(Name = "Đồng ý với điều khoản")]
        [MustBeTrue(ErrorMessage = "Bạn phải đồng ý với điều khoản sử dụng")]
        public bool AgreeToTerms { get; set; }
    }

    public class MustBeTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is bool boolValue && boolValue;
        }
    }
}
