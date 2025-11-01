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
        private readonly CartService _cartService;
        private readonly OrderService _orderService;
        private readonly AddressService _addressService;
        private readonly EmailService _emailService;
        private readonly VNPayService _vnpayService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(CartService cartService, OrderService orderService, AddressService addressService, EmailService emailService, VNPayService vnpayService, IConfiguration configuration, ILogger<CheckoutController> logger)
        {
            _cartService = cartService;
            _orderService = orderService;
            _addressService = addressService;
            _emailService = emailService;
            _vnpayService = vnpayService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Thanh toán";
                
                var userId = GetCurrentUserId();
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                var addresses = await _addressService.GetUserAddressesAsync(userId);
                var paymentMethods = GetPaymentMethods();
                
                if (!cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.";
                    return RedirectToAction("Index", "Cart");
                }
                
                var subtotal = cartItems.Sum(item => item.TotalPrice);
                var shippingFee = subtotal > 500000 ? 0 : 30000;
                var discountAmount = 0m;
                var totalAmount = subtotal + shippingFee - discountAmount;
                
                // Initialize model with default values
                var model = new CheckoutViewModel
                {
                    AddressId = addresses?.FirstOrDefault(a => a.IsDefault)?.AddressID, // Default address
                    PaymentMethodId = 1, // Default to COD
                    AgreeToTerms = false
                };
                
                ViewBag.CartItems = cartItems;
                ViewBag.Addresses = addresses;
                ViewBag.PaymentMethods = paymentMethods;
                ViewBag.Subtotal = subtotal;
                ViewBag.ShippingFee = shippingFee;
                ViewBag.DiscountAmount = discountAmount;
                ViewBag.TotalAmount = totalAmount;
                
                return View(model);
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
                // Debug logging
                _logger.LogInformation($"ProcessOrder received - AddressId: {model.AddressId}, PaymentMethodId: {model.PaymentMethodId}, AgreeToTerms: {model.AgreeToTerms}");
                
                if (!ModelState.IsValid)
                {
                    // Log validation errors
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning($"ModelState invalid: {string.Join(", ", errors)}");
                    
                    // Reload checkout page with errors
                    TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin đơn hàng. " + string.Join(", ", errors);
                    return RedirectToAction("Index");
                }

                var userId = GetCurrentUserId();
                
                // Create order
                var order = await _orderService.CreateOrderAsync(userId, model);
                
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Không thể tạo đơn hàng. Vui lòng kiểm tra lại giỏ hàng.";
                    return RedirectToAction("Index");
                }
                
                // Handle payment method
                if (model.PaymentMethodId == 2) // VNPay
                {
                    var returnUrl = Url.Action("PaymentReturn", "Checkout", new { orderNumber = order.OrderNumber }, Request.Scheme);
                    var paymentUrl = _vnpayService.CreatePaymentUrl(order.OrderNumber, order.TotalAmount, returnUrl);
                    return Redirect(paymentUrl);
                }
                else // COD
                {
                    TempData["SuccessMessage"] = "Đặt hàng thành công! Mã đơn hàng: " + order.OrderNumber;
                    return RedirectToAction("OrderConfirmation", new { orderNumber = order.OrderNumber });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing order: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xử lý đơn hàng. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(string orderNumber)
        {
            try
            {
                ViewData["Title"] = "Xác nhận đơn hàng";
                
                var order = await _orderService.GetOrderByNumberAsync(orderNumber);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                    return RedirectToAction("Index", "Home");
                }
                
                var orderDetails = new OrderConfirmationViewModel
                {
                    OrderNumber = order.OrderNumber,
                    OrderDate = order.OrderDate,
                    Status = order.Status.StatusName,
                    EstimatedDelivery = order.OrderDate.AddDays(3),
                    TotalAmount = order.TotalAmount
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

        [HttpGet]
        public async Task<IActionResult> PaymentReturn(string orderNumber)
        {
            try
            {
                // Get VNPay response parameters
                var vnpParams = new Dictionary<string, string>();
                foreach (var key in Request.Query.Keys)
                {
                    vnpParams.Add(key, Request.Query[key].ToString());
                }

                // Process VNPay response
                var paymentResult = _vnpayService.ProcessPaymentResponse(vnpParams);
                
                if (paymentResult.IsSuccess)
                {
                    // Update order status to paid
                    var order = await _orderService.GetOrderByNumberAsync(orderNumber);
                    if (order != null)
                    {
                        await _orderService.UpdateOrderStatusAsync(order.OrderID, 2, GetCurrentUserId(), "Thanh toán VNPay thành công");
                    }
                    
                    TempData["SuccessMessage"] = "Thanh toán thành công! Đơn hàng đã được xác nhận.";
                    return RedirectToAction("OrderConfirmation", new { orderNumber = orderNumber });
                }
                else
                {
                    TempData["ErrorMessage"] = "Thanh toán thất bại. Vui lòng thử lại hoặc chọn phương thức thanh toán khác.";
                    return RedirectToAction("Index", "Cart");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing VNPay return for order {orderNumber}: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xử lý thanh toán.";
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PaymentIPN()
        {
            try
            {
                // Get VNPay IPN parameters
                var vnpParams = new Dictionary<string, string>();
                foreach (var key in Request.Form.Keys)
                {
                    vnpParams.Add(key, Request.Form[key].ToString());
                }

                // Process IPN
                var paymentResult = _vnpayService.ProcessPaymentResponse(vnpParams);
                
                if (paymentResult.IsSuccess)
                {
                    var order = await _orderService.GetOrderByNumberAsync(paymentResult.OrderId);
                    if (order != null)
                    {
                        await _orderService.UpdateOrderStatusAsync(order.OrderID, 2, GetCurrentUserId(), "IPN: Thanh toán VNPay thành công");
                    }
                }

                return Ok("OK");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing VNPay IPN: {ex.Message}");
                return BadRequest("Error");
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
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
