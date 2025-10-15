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

        public HomeController(ILogger<HomeController> logger, CategoryService categoryService, BannerService bannerService, ProductService productService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _bannerService = bannerService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                var banners = await _bannerService.GetActiveBannersAsync();
                var featuredProducts = await _productService.GetFeaturedProductsAsync(); // Lấy sản phẩm nổi bật
                
                ViewBag.Categories = categories;
                ViewBag.Banners = banners;
                ViewBag.FeaturedProducts = featuredProducts;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading data for home page: {ex.Message}");
                ViewBag.Categories = new List<Pet_Shop.Models.Entities.Category>();
                ViewBag.Banners = new List<Pet_Shop.Models.Entities.Banner>();
                ViewBag.FeaturedProducts = new List<Pet_Shop.Models.Entities.Product>();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
