using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pet_Shop.Services;
using System.Security.Claims;

namespace Pet_Shop.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly WishlistService _wishlistService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(WishlistService wishlistService, ILogger<WishlistController> logger)
        {
            _wishlistService = wishlistService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Danh sách yêu thích";
                
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                var wishlistItems = await _wishlistService.GetUserWishlistAsync(userId);
                return View(wishlistItems);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading wishlist: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách yêu thích.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thêm vào yêu thích." });
                }

                var success = await _wishlistService.AddToWishlistAsync(userId, productId);
                if (success)
                {
                    var wishlistCount = await _wishlistService.GetWishlistCountAsync(userId);
                    return Json(new { 
                        success = true, 
                        message = "Đã thêm vào danh sách yêu thích",
                        wishlistCount = wishlistCount
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Sản phẩm đã có trong danh sách yêu thích hoặc không khả dụng." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding to wishlist: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm vào yêu thích." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập." });
                }

                var success = await _wishlistService.RemoveFromWishlistAsync(wishlistId, userId);
                if (success)
                {
                    var wishlistCount = await _wishlistService.GetWishlistCountAsync(userId);
                    return Json(new { 
                        success = true, 
                        message = "Đã xóa khỏi danh sách yêu thích",
                        wishlistCount = wishlistCount
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa khỏi danh sách yêu thích." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing from wishlist: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa khỏi yêu thích." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlistByProduct(int productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập." });
                }

                var success = await _wishlistService.RemoveFromWishlistByProductAsync(userId, productId);
                if (success)
                {
                    var wishlistCount = await _wishlistService.GetWishlistCountAsync(userId);
                    return Json(new { 
                        success = true, 
                        message = "Đã xóa khỏi danh sách yêu thích",
                        wishlistCount = wishlistCount
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa khỏi danh sách yêu thích." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing from wishlist: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa khỏi yêu thích." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> IsInWishlist(int productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Json(new { success = false, inWishlist = false });
                }

                var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, productId);
                return Json(new { success = true, inWishlist = isInWishlist });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking wishlist status: {ex.Message}");
                return Json(new { success = false, inWishlist = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlistCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Json(new { success = false, count = 0 });
                }

                var count = await _wishlistService.GetWishlistCountAsync(userId);
                return Json(new { success = true, count = count });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting wishlist count: {ex.Message}");
                return Json(new { success = false, count = 0 });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
