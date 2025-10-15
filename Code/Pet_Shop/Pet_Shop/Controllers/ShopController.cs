using Microsoft.AspNetCore.Mvc;
using Pet_Shop.Services;
using Pet_Shop.Models.Entities;

namespace Pet_Shop.Controllers
{
    public class ShopController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly ILogger<ShopController> _logger;

        public ShopController(ProductService productService, CategoryService categoryService, ILogger<ShopController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Products(int? categoryId, int? brandId, decimal? minPrice, decimal? maxPrice, string? sortBy, int page = 1)
        {
            try
            {
                ViewData["Title"] = "Cửa hàng sản phẩm";
                
                // Get filter options
                var categories = await _categoryService.GetAllCategoriesAsync();
                var brands = await _productService.GetAllBrandsAsync();
                
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                ViewBag.SelectedCategoryId = categoryId;
                ViewBag.SelectedBrandId = brandId;
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;
                ViewBag.SortBy = sortBy;
                ViewBag.CurrentPage = page;

                // Get products
                IEnumerable<Product> products = await _productService.GetAllProductsAsync();

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
                ViewBag.Products = products;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading products: {ex.Message}");
                ViewBag.Error = "Có lỗi xảy ra khi tải danh sách sản phẩm";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Category(int id, int page = 1)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                ViewData["Title"] = $"Danh mục: {category.CategoryName}";
                
                // Get products for this category
                var products = await _productService.GetProductsByCategoryAsync(id);
                
                // Pagination
                const int pageSize = 12;
                var totalItems = products.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                
                products = products.Skip((page - 1) * pageSize).Take(pageSize);

                ViewBag.Category = category;
                ViewBag.Products = products;
                ViewBag.TotalItems = totalItems;
                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = page;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading category {id}: {ex.Message}");
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Product(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                ViewData["Title"] = product.ProductName;
                
                // Get related products (same category)
                var relatedProducts = await _productService.GetRelatedProductsAsync(id, product.CategoryID, 4);
                
                ViewBag.Product = product;
                ViewBag.RelatedProducts = relatedProducts;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading product {id}: {ex.Message}");
                return NotFound();
            }
        }
    }
}
