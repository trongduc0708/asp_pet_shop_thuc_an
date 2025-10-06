using Microsoft.AspNetCore.Mvc;
using Pet_Shop.Services;

namespace Pet_Shop.Controllers
{
    public class DemoController : Controller
    {
        private readonly ProductService _productService;

        public DemoController(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Products()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                var categories = await _productService.GetAllCategoriesAsync();
                var brands = await _productService.GetAllBrandsAsync();
                var featuredProducts = await _productService.GetFeaturedProductsAsync();
                var newProducts = await _productService.GetNewProductsAsync();
                var statistics = await _productService.GetProductStatisticsAsync();

                ViewBag.Products = products;
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                ViewBag.FeaturedProducts = featuredProducts;
                ViewBag.NewProducts = newProducts;
                ViewBag.Statistics = statistics;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return RedirectToAction("Products");
            }

            try
            {
                var products = await _productService.SearchProductsAsync(searchTerm);
                ViewBag.Products = products;
                ViewBag.SearchTerm = searchTerm;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
    }
}
