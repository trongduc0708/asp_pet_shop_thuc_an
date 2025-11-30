using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Pet_Shop.Models;
using Pet_Shop.Services;
using Pet_Shop.Data;

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
        private readonly LocalRecommendationService? _localRecommendationService;
        private readonly IConfiguration _configuration;
        private readonly PetShopDbContext _context;

        public HomeController(
            ILogger<HomeController> logger, 
            CategoryService categoryService, 
            BannerService bannerService, 
            ProductService productService, 
            PromotionService promotionService,
            ChatbotService chatbotService,
            LocalRecommendationService? localRecommendationService,
            IConfiguration configuration,
            PetShopDbContext context)
        {
            _logger = logger;
            _categoryService = categoryService;
            _bannerService = bannerService;
            _productService = productService;
            _promotionService = promotionService;
            _chatbotService = chatbotService;
            _localRecommendationService = localRecommendationService;
            _configuration = configuration;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                var banners = await _bannerService.GetActiveBannersAsync();
                var activePromotions = await _promotionService.GetActivePromotionsAsync(); // Lấy mã khuyến mãi đang hoạt động
                
                var featuredResult = await GetHomepageFeaturedProductsAsync(); // Lấy sản phẩm nổi bật/đề xuất từ Local ML
                
                // AI Gợi ý: Sử dụng Local ML API (LightFM + Sentence-BERT Hybrid) thay vì ChatbotService
                List<Pet_Shop.Models.Entities.Product>? aiRecommendedProducts = null;
                bool localMLNoData = false;
                var userId = GetCurrentUserId();
                if (userId > 0)
                {
                    var result = await GetAIRecommendedProductsAsync(userId, 8);
                    aiRecommendedProducts = result.products;
                    localMLNoData = result.noData;
                    _logger.LogInformation($"AI Recommendations loaded {aiRecommendedProducts?.Count ?? 0} products for user {userId} from Local ML API. No data: {localMLNoData}");
                }
                
                ViewBag.Categories = categories;
                ViewBag.Banners = banners;
                ViewBag.FeaturedProducts = featuredResult.products;
                ViewBag.FeaturedProductsNoData = featuredResult.noData;
                ViewBag.ActivePromotions = activePromotions;
                ViewBag.AIRecommendedProducts = aiRecommendedProducts;
                ViewBag.LocalMLNoData = localMLNoData;
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

        private async Task<(List<Pet_Shop.Models.Entities.Product> products, bool noData)> GetHomepageFeaturedProductsAsync()
        {
            var featuredProducts = new List<Pet_Shop.Models.Entities.Product>();
            bool noData = false;
            
            try
            {
                var useLocalML = _configuration.GetValue<bool>("LocalMLSettings:Enabled", false);
                if (useLocalML && _localRecommendationService != null)
                {
                    var localMLAvailable = await _localRecommendationService.IsApiAvailableAsync();
                    if (localMLAvailable)
                    {
                        var userId = GetCurrentUserId();

                        if (userId > 0)
                        {
                            var numInteractions = await GetUserInteractionCountAsync(userId);
                            featuredProducts = await _localRecommendationService.GetRecommendedProductsAsync(
                                userId, 
                                8, 
                                currentProductId: null, 
                                numInteractions: numInteractions
                            );
                        }

                        if (!featuredProducts.Any())
                        {
                            featuredProducts = await _localRecommendationService.GetRecommendedProductsAsync(0, 8);
                        }

                        if (!featuredProducts.Any())
                        {
                            noData = true;
                            _logger.LogWarning("LocalRecommendationService returned no data for featured products");
                        }
                    }
                    else
                    {
                        noData = true;
                        _logger.LogWarning("LocalRecommendationService API not available - no data");
                    }
                }
                else
                {
                    noData = true;
                    _logger.LogWarning("LocalRecommendationService not enabled or not available - no data");
                }
            }
            catch (Exception ex)
            {
                noData = true;
                _logger.LogWarning(ex, "Error getting featured products from Local ML service - no data");
            }

            // Không fallback về database - chỉ dùng LocalRecommendationService
            return (featuredProducts, noData);
        }

        /// <summary>
        /// Lấy AI recommendations từ Local ML API (LightFM + Sentence-BERT Hybrid)
        /// </summary>
        private async Task<(List<Pet_Shop.Models.Entities.Product> products, bool noData)> GetAIRecommendedProductsAsync(int userId, int count = 8)
        {
            var aiRecommendedProducts = new List<Pet_Shop.Models.Entities.Product>();
            bool noData = false;
            
            try
            {
                var useLocalML = _configuration.GetValue<bool>("LocalMLSettings:Enabled", false);
                if (useLocalML && _localRecommendationService != null)
                {
                    var localMLAvailable = await _localRecommendationService.IsApiAvailableAsync();
                    if (localMLAvailable)
                    {
                        // Đếm số interactions của user (từ OrderItems) để tính dynamic weighting
                        var numInteractions = await GetUserInteractionCountAsync(userId);
                        
                        // Gọi Local ML API với num_interactions để dynamic weighting
                        aiRecommendedProducts = await _localRecommendationService.GetRecommendedProductsAsync(
                            userId, 
                            count, 
                            currentProductId: null, 
                            numInteractions: numInteractions
                        );
                        
                        if (!aiRecommendedProducts.Any())
                        {
                            noData = true;
                            _logger.LogWarning($"LocalRecommendationService returned no data for user {userId}");
                        }
                        else
                        {
                            _logger.LogInformation($"Local ML API returned {aiRecommendedProducts.Count} recommendations for user {userId} (interactions: {numInteractions})");
                        }
                    }
                    else
                    {
                        noData = true;
                        _logger.LogWarning("LocalRecommendationService API not available - no data");
                    }
                }
                else
                {
                    noData = true;
                    _logger.LogWarning("LocalRecommendationService not enabled or not available - no data");
                }
            }
            catch (Exception ex)
            {
                noData = true;
                _logger.LogWarning(ex, "Error getting AI recommendations from Local ML service - no data");
            }

            // Không fallback - chỉ dùng LocalRecommendationService
            return (aiRecommendedProducts, noData);
        }

        /// <summary>
        /// Đếm số interactions của user (từ OrderItems) để tính dynamic weighting
        /// </summary>
        private async Task<int> GetUserInteractionCountAsync(int userId)
        {
            try
            {
                // Đếm số OrderItems của user (không tính đơn hàng đã hủy)
                var interactionCount = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .Where(oi => oi.Order.UserID == userId && oi.Order.StatusID != 5)
                    .CountAsync();
                
                return interactionCount;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error counting user interactions for user {userId}");
                return 0;
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
