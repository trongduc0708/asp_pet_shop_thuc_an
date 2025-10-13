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
        private readonly ProductService _productService;
        private readonly ILogger<CartController> _logger;

        public CartController(ProductService productService, ILogger<CartController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                ViewData["Title"] = "Giỏ hàng";
                
                // TODO: Get cart items from database
                // For now, return empty cart
                var cartItems = new List<CartItemViewModel>();
                ViewBag.CartTotal = 0;
                ViewBag.CartCount = 0;
                
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
                // TODO: Implement add to cart logic
                // 1. Check if product exists
                // 2. Check if user is authenticated
                // 3. Add or update cart item
                
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại" });
                }

                // TODO: Add to database cart
                // For now, just return success
                
                return Json(new { 
                    success = true, 
                    message = "Đã thêm sản phẩm vào giỏ hàng",
                    cartCount = 1 // TODO: Get actual cart count
                });
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
                // TODO: Implement update quantity logic
                if (quantity <= 0)
                {
                    return Json(new { success = false, message = "Số lượng phải lớn hơn 0" });
                }

                // TODO: Update cart item quantity in database
                
                return Json(new { 
                    success = true, 
                    message = "Đã cập nhật số lượng",
                    newQuantity = quantity
                });
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
                // TODO: Implement remove item logic
                // Remove item from database cart
                
                return Json(new { 
                    success = true, 
                    message = "Đã xóa sản phẩm khỏi giỏ hàng"
                });
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
                // TODO: Implement clear cart logic
                // Clear all items from database cart
                
                return Json(new { 
                    success = true, 
                    message = "Đã xóa tất cả sản phẩm khỏi giỏ hàng"
                });
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
                // TODO: Get actual cart count from database
                var cartCount = 0;
                
                return Json(new { cartCount = cartCount });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting cart count: {ex.Message}");
                return Json(new { cartCount = 0 });
            }
        }
    }

}
