using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pet_Shop.Services;
using Pet_Shop.Models.Entities;
using Pet_Shop.Models.ViewModels;
using System.Security.Claims;

namespace Pet_Shop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(CartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Giỏ hàng";
                
                var userId = GetCurrentUserId();
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                var cartTotal = await _cartService.GetCartTotalAsync(userId);
                var cartCount = await _cartService.GetCartCountAsync(userId);
                
                ViewBag.CartTotal = cartTotal;
                ViewBag.CartCount = cartCount;
                
                return View(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading cart: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải giỏ hàng.";
                return View(new List<CartItemViewModel>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _cartService.AddToCartAsync(userId, productId, quantity);
                
                if (success)
                {
                    var cartCount = await _cartService.GetCartCountAsync(userId);
                    return Json(new { 
                        success = true, 
                        message = "Đã thêm sản phẩm vào giỏ hàng",
                        cartCount = cartCount
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể thêm sản phẩm vào giỏ hàng. Có thể sản phẩm không còn hàng." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding to cart: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm vào giỏ hàng" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _cartService.UpdateQuantityAsync(userId, productId, quantity);
                
                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = "Đã cập nhật số lượng",
                        newQuantity = quantity
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể cập nhật số lượng. Có thể sản phẩm không còn đủ hàng." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating cart quantity: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật số lượng" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _cartService.RemoveItemAsync(userId, productId);
                
                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = "Đã xóa sản phẩm khỏi giỏ hàng"
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa sản phẩm khỏi giỏ hàng" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing cart item: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa sản phẩm" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _cartService.ClearCartAsync(userId);
                
                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = "Đã xóa tất cả sản phẩm khỏi giỏ hàng"
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa giỏ hàng" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error clearing cart: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa giỏ hàng" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cartCount = await _cartService.GetCartCountAsync(userId);
                
                return Json(new { cartCount = cartCount });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting cart count: {ex.Message}");
                return Json(new { cartCount = 0 });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }

}
