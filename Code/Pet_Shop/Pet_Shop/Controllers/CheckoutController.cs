using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pet_Shop.Services;
using Pet_Shop.Models.Entities;
using Pet_Shop.Models.ViewModels;
using System.Security.Claims;

namespace Pet_Shop.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ProductService _productService;
        private readonly EmailService _emailService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(ProductService productService, EmailService emailService, ILogger<CheckoutController> logger)
        {
            _productService = productService;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Thanh toán";
                
                // TODO: Get cart items from database
                var cartItems = new List<CartItemViewModel>();
                var addresses = new List<AddressViewModel>();
                var paymentMethods = GetPaymentMethods();
                
                ViewBag.CartItems = cartItems;
                ViewBag.Addresses = addresses;
                ViewBag.PaymentMethods = paymentMethods;
                ViewBag.Subtotal = 0;
                ViewBag.ShippingFee = 0;
                ViewBag.DiscountAmount = 0;
                ViewBag.TotalAmount = 0;
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading checkout: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải trang thanh toán.";
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder(CheckoutViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Reload checkout page with errors
                    return await Index();
                }

                // TODO: Implement order processing
                // 1. Validate cart items
                // 2. Check stock availability
                // 3. Calculate totals
                // 4. Create order
                // 5. Process payment
                // 6. Send confirmation email
                // 7. Clear cart

                var orderNumber = GenerateOrderNumber();
                
                // TODO: Save order to database
                // TODO: Process payment
                // TODO: Send confirmation email
                
                TempData["SuccessMessage"] = "Đặt hàng thành công! Mã đơn hàng: " + orderNumber;
                return RedirectToAction("OrderConfirmation", new { orderNumber = orderNumber });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing order: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xử lý đơn hàng. Vui lòng thử lại.";
                return await Index();
            }
        }

        [HttpGet]
        public IActionResult OrderConfirmation(string orderNumber)
        {
            try
            {
                ViewData["Title"] = "Xác nhận đơn hàng";
                ViewBag.OrderNumber = orderNumber;
                
                // TODO: Get order details from database
                var orderDetails = new OrderConfirmationViewModel
                {
                    OrderNumber = orderNumber,
                    OrderDate = DateTime.Now,
                    Status = "Đang xử lý",
                    EstimatedDelivery = DateTime.Now.AddDays(3),
                    TotalAmount = 0
                };
                
                return View(orderDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading order confirmation: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin đơn hàng.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ValidatePromoCode(string promoCode)
        {
            try
            {
                if (string.IsNullOrEmpty(promoCode))
                {
                    return Json(new { success = false, message = "Vui lòng nhập mã giảm giá" });
                }

                // TODO: Validate promo code in database
                // For now, return mock validation
                if (promoCode.ToUpper() == "WELCOME10")
                {
                    return Json(new { 
                        success = true, 
                        message = "Áp dụng mã giảm giá thành công",
                        discountAmount = 50000,
                        discountType = "percentage",
                        discountValue = 10
                    });
                }
                else if (promoCode.ToUpper() == "SAVE50K")
                {
                    return Json(new { 
                        success = true, 
                        message = "Áp dụng mã giảm giá thành công",
                        discountAmount = 50000,
                        discountType = "fixed",
                        discountValue = 50000
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating promo code: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi kiểm tra mã giảm giá" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CalculateShipping(string city, string district)
        {
            try
            {
                // TODO: Calculate shipping fee based on location
                // For now, return mock calculation
                var shippingFee = 30000; // Default 30k
                
                if (city?.ToLower().Contains("hồ chí minh") == true || 
                    city?.ToLower().Contains("hà nội") == true)
                {
                    shippingFee = 20000; // 20k for major cities
                }
                
                return Json(new { 
                    success = true, 
                    shippingFee = shippingFee,
                    estimatedDays = 1
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating shipping: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi tính phí vận chuyển" });
            }
        }

        private string GenerateOrderNumber()
        {
            return "PS" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private List<PaymentMethodViewModel> GetPaymentMethods()
        {
            return new List<PaymentMethodViewModel>
            {
                new PaymentMethodViewModel
                {
                    Id = 1,
                    Name = "Thanh toán khi nhận hàng (COD)",
                    Description = "Thanh toán bằng tiền mặt khi nhận hàng",
                    Icon = "fas fa-money-bill-wave",
                    IsActive = true
                },
                new PaymentMethodViewModel
                {
                    Id = 2,
                    Name = "Thanh toán qua VNPay",
                    Description = "Thanh toán online qua VNPay",
                    Icon = "fas fa-credit-card",
                    IsActive = true
                }
            };
        }
    }
}
