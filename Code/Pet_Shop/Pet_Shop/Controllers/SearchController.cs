using Microsoft.AspNetCore.Mvc;
using Pet_Shop.Services;
using Pet_Shop.Models.Entities;

namespace Pet_Shop.Controllers
{
    public class SearchController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly ILogger<SearchController> _logger;

        public SearchController(ProductService productService, CategoryService categoryService, ILogger<SearchController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, int? categoryId, int? brandId, decimal? minPrice, decimal? maxPrice, string? sortBy, int page = 1)
        {
            try
            {
                ViewData["Title"] = "Tìm kiếm sản phẩm";
                
                // Get filter options
                var categories = await _categoryService.GetAllCategoriesAsync();
                var brands = await _productService.GetAllBrandsAsync();
                
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                ViewBag.SearchQuery = q;
                ViewBag.SelectedCategoryId = categoryId;
                ViewBag.SelectedBrandId = brandId;
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;
                ViewBag.SortBy = sortBy;
                ViewBag.CurrentPage = page;

                // Search products
                IEnumerable<Product> products;
                
                if (!string.IsNullOrEmpty(q))
                {
                    products = await _productService.SearchProductsAsync(q);
                }
                else
                {
                    products = await _productService.GetAllProductsAsync();
                }

                // Apply filters
                if (categoryId.HasValue)
                {
                    products = products.Where(p => p.CategoryID == categoryId.Value);
                }

                if (brandId.HasValue)
                {
                    products = products.Where(p => p.BrandID == brandId.Value);
                }

                if (minPrice.HasValue)
                {
                    products = products.Where(p => p.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    products = products.Where(p => p.Price <= maxPrice.Value);
                }

                // Apply sorting
                switch (sortBy)
                {
                    case "name_asc":
                        products = products.OrderBy(p => p.ProductName);
                        break;
                    case "name_desc":
                        products = products.OrderByDescending(p => p.ProductName);
                        break;
                    case "price_asc":
                        products = products.OrderBy(p => p.Price);
                        break;
                    case "price_desc":
                        products = products.OrderByDescending(p => p.Price);
                        break;
                    case "newest":
                        products = products.OrderByDescending(p => p.CreatedDate);
                        break;
                    default:
                        products = products.OrderBy(p => p.ProductName);
                        break;
                }

                // Pagination
                const int pageSize = 12;
                var totalItems = products.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                
                products = products.Skip((page - 1) * pageSize).Take(pageSize);

                ViewBag.TotalItems = totalItems;
                ViewBag.TotalPages = totalPages;
                ViewBag.HasPreviousPage = page > 1;
                ViewBag.HasNextPage = page < totalPages;

                return View(products.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in search: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tìm kiếm sản phẩm.";
                return View(new List<Product>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> QuickSearch(string term)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    return Json(new { success = false, message = "Vui lòng nhập từ khóa tìm kiếm" });
                }

                var products = await _productService.SearchProductsAsync(term);
                var results = products.Take(5).Select(p => new
                {
                    id = p.ProductID,
                    name = p.ProductName,
                    price = p.Price,
                    image = p.ProductImages.FirstOrDefault()?.ImageURL ?? "/images/no-image.jpg",
                    url = Url.Action("Details", "Product", new { id = p.ProductID })
                });

                return Json(new { success = true, results = results });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in quick search: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi tìm kiếm" });
            }
        }
    }
}
