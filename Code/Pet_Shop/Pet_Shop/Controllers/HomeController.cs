using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Pet_Shop.Models;
using Pet_Shop.Services;

namespace Pet_Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CategoryService _categoryService;
        private readonly BannerService _bannerService;
        private readonly ProductService _productService;
        private readonly PromotionService _promotionService;
        private readonly ChatbotService _chatbotService;

        public HomeController(
            ILogger<HomeController> logger, 
            CategoryService categoryService, 
            BannerService bannerService, 
            ProductService productService, 
            PromotionService promotionService,
            ChatbotService chatbotService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _bannerService = bannerService;
            _productService = productService;
            _promotionService = promotionService;
            _chatbotService = chatbotService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                var banners = await _bannerService.GetActiveBannersAsync();
                var featuredProducts = await _productService.GetFeaturedProductsAsync(); // Lấy sản phẩm nổi bật
                var activePromotions = await _promotionService.GetActivePromotionsAsync(); // Lấy mã khuyến mãi đang hoạt động
                
                // Chatbot AI: Gợi ý sản phẩm dựa trên lịch sử mua hàng của khách hàng đã đăng nhập
                List<Pet_Shop.Models.Entities.Product>? aiRecommendedProducts = null;
                var userId = GetCurrentUserId();
                if (userId > 0)
                {
                    aiRecommendedProducts = await _chatbotService.GetRecommendedProductsForUserAsync(userId, 8);
                    _logger.LogInformation($"AI Chatbot loaded {aiRecommendedProducts.Count} recommendations for user {userId}");
                }
                
                ViewBag.Categories = categories;
                ViewBag.Banners = banners;
                ViewBag.FeaturedProducts = featuredProducts;
                ViewBag.ActivePromotions = activePromotions;
                ViewBag.AIRecommendedProducts = aiRecommendedProducts;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading data for home page: {ex.Message}");
                ViewBag.Categories = new List<Pet_Shop.Models.Entities.Category>();
                ViewBag.Banners = new List<Pet_Shop.Models.Entities.Banner>();
                ViewBag.FeaturedProducts = new List<Pet_Shop.Models.Entities.Product>();
                ViewBag.ActivePromotions = new List<Pet_Shop.Models.Entities.Promotion>();
                ViewBag.AIRecommendedProducts = null;
                return View();
            }
        }

        private int GetCurrentUserId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            return 0;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        /// <summary>
        /// API endpoint để lấy danh sách mã khuyến mãi đang hoạt động
        /// </summary>
        /// <returns>JSON response với danh sách mã khuyến mãi</returns>
        [HttpGet]
        public async Task<IActionResult> GetActivePromotions()
        {
            try
            {
                var promotions = await _promotionService.GetActivePromotionsAsync();
                return Json(new { success = true, data = promotions });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting active promotions: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi tải mã khuyến mãi" });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
