using System.Diagnostics;
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

        public HomeController(ILogger<HomeController> logger, CategoryService categoryService, BannerService bannerService, ProductService productService, PromotionService promotionService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _bannerService = bannerService;
            _productService = productService;
            _promotionService = promotionService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                var banners = await _bannerService.GetActiveBannersAsync();
                var featuredProducts = await _productService.GetFeaturedProductsAsync(); // Lấy sản phẩm nổi bật
                var activePromotions = await _promotionService.GetActivePromotionsAsync(); // Lấy mã khuyến mãi đang hoạt động
                
                ViewBag.Categories = categories;
                ViewBag.Banners = banners;
                ViewBag.FeaturedProducts = featuredProducts;
                ViewBag.ActivePromotions = activePromotions;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading data for home page: {ex.Message}");
                ViewBag.Categories = new List<Pet_Shop.Models.Entities.Category>();
                ViewBag.Banners = new List<Pet_Shop.Models.Entities.Banner>();
                ViewBag.FeaturedProducts = new List<Pet_Shop.Models.Entities.Product>();
                ViewBag.ActivePromotions = new List<Pet_Shop.Models.Entities.Promotion>();
                return View();
            }
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
