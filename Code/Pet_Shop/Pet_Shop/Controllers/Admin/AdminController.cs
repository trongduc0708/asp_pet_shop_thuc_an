using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pet_Shop.Services;
using System.Security.Claims;

namespace Pet_Shop.Controllers.Admin
{
    [Authorize(Roles = "Admin,Employee")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;
        private readonly BannerService _bannerService;

        public AdminController(ILogger<AdminController> logger, CategoryService categoryService, ProductService productService, BannerService bannerService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _productService = productService;
            _bannerService = bannerService;
        }

        [HttpGet]
        [Route("")]
        [Route("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                var categoryCount = categories.Count();

                // Get user info
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = User.FindFirstValue(ClaimTypes.Name);
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                ViewBag.CategoryCount = categoryCount;
                ViewBag.UserName = userName;
                ViewBag.UserRole = userRole;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading admin dashboard: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải trang quản trị.";
                return View();
            }
        }

        [HttpGet]
        [Route("Categories")]
        public async Task<IActionResult> Categories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading categories: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách danh mục.";
                return View(new List<Pet_Shop.Models.Entities.Category>());
            }
        }

        [HttpGet]
        [Route("Categories/Create")]
        public async Task<IActionResult> CreateCategory()
        {
            try
            {
                var parentCategories = await _categoryService.GetParentCategoriesAsync();
                ViewBag.ParentCategories = parentCategories;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading create category form: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form tạo danh mục.";
                return RedirectToAction("Categories");
            }
        }

        [HttpPost]
        [Route("Categories/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Pet_Shop.Models.Entities.Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var parentCategories = await _categoryService.GetParentCategoriesAsync();
                    ViewBag.ParentCategories = parentCategories;
                    return View(category);
                }

                // Check if category name already exists
                var exists = await _categoryService.CategoryExistsAsync(category.CategoryName, category.ParentCategoryID);
                if (exists)
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại.");
                    var parentCategories = await _categoryService.GetParentCategoriesAsync();
                    ViewBag.ParentCategories = parentCategories;
                    return View(category);
                }

                category.IsActive = true;
                category.CreatedDate = DateTime.Now;

                var success = await _categoryService.CreateCategoryAsync(category);
                if (success)
                {
                    TempData["SuccessMessage"] = "Tạo danh mục thành công!";
                    return RedirectToAction("Categories");
                }

                ModelState.AddModelError(string.Empty, "Tạo danh mục thất bại.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating category: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi tạo danh mục.");
            }

            var parentCategoriesForError = await _categoryService.GetParentCategoriesAsync();
            ViewBag.ParentCategories = parentCategoriesForError;
            return View(category);
        }

        [HttpGet]
        [Route("Categories/Edit/{id}")]
        public async Task<IActionResult> EditCategory(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy danh mục.";
                    return RedirectToAction("Categories");
                }

                var parentCategories = await _categoryService.GetParentCategoriesAsync();
                ViewBag.ParentCategories = parentCategories;
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading edit category form: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form chỉnh sửa danh mục.";
                return RedirectToAction("Categories");
            }
        }

        [HttpPost]
        [Route("Categories/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Pet_Shop.Models.Entities.Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var parentCategories = await _categoryService.GetParentCategoriesAsync();
                    ViewBag.ParentCategories = parentCategories;
                    return View(category);
                }

                // Check if category name already exists (excluding current category)
                var exists = await _categoryService.CategoryExistsAsync(category.CategoryName, category.ParentCategoryID, category.CategoryID);
                if (exists)
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại.");
                    var parentCategories = await _categoryService.GetParentCategoriesAsync();
                    ViewBag.ParentCategories = parentCategories;
                    return View(category);
                }

                var success = await _categoryService.UpdateCategoryAsync(category);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                    return RedirectToAction("Categories");
                }

                ModelState.AddModelError(string.Empty, "Cập nhật danh mục thất bại.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating category: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật danh mục.");
            }

            var parentCategoriesForError = await _categoryService.GetParentCategoriesAsync();
            ViewBag.ParentCategories = parentCategoriesForError;
            return View(category);
        }

        [HttpPost]
        [Route("Categories/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var success = await _categoryService.DeleteCategoryAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Xóa danh mục thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa danh mục.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting category: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa danh mục.";
            }

            return RedirectToAction("Categories");
        }

        [HttpGet]
        [Route("Categories/Search")]
        public async Task<IActionResult> SearchCategories(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return RedirectToAction("Categories");
                }

                var categories = await _categoryService.SearchCategoriesAsync(searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View("Categories", categories);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching categories: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tìm kiếm danh mục.";
                return RedirectToAction("Categories");
            }
        }

        [HttpGet]
        [Route("Banners")]
        public async Task<IActionResult> Banners()
        {
            try
            {
                var banners = await _bannerService.GetAllBannersAsync();
                return View(banners);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading banners: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách banner.";
                return View(new List<Pet_Shop.Models.Entities.Banner>());
            }
        }

        [HttpGet]
        [Route("Banners/Create")]
        public IActionResult CreateBanner()
        {
            return View();
        }

        [HttpPost]
        [Route("Banners/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBanner(Pet_Shop.Models.Entities.Banner banner)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(banner);
                }

                // Check if banner name already exists
                var exists = await _bannerService.BannerExistsAsync(banner.BannerName);
                if (exists)
                {
                    ModelState.AddModelError("BannerName", "Tên banner đã tồn tại.");
                    return View(banner);
                }

                banner.IsActive = true;
                banner.CreatedDate = DateTime.Now;

                var success = await _bannerService.CreateBannerAsync(banner);
                if (success)
                {
                    TempData["SuccessMessage"] = "Tạo banner thành công!";
                    return RedirectToAction("Banners");
                }

                ModelState.AddModelError(string.Empty, "Tạo banner thất bại.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating banner: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi tạo banner.");
            }

            return View(banner);
        }

        [HttpGet]
        [Route("Banners/Edit/{id}")]
        public async Task<IActionResult> EditBanner(int id)
        {
            try
            {
                var banner = await _bannerService.GetBannerByIdAsync(id);
                if (banner == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy banner.";
                    return RedirectToAction("Banners");
                }

                return View(banner);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading edit banner form: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form chỉnh sửa banner.";
                return RedirectToAction("Banners");
            }
        }

        [HttpPost]
        [Route("Banners/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBanner(Pet_Shop.Models.Entities.Banner banner)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(banner);
                }

                // Check if banner name already exists (excluding current banner)
                var exists = await _bannerService.BannerExistsAsync(banner.BannerName, banner.BannerID);
                if (exists)
                {
                    ModelState.AddModelError("BannerName", "Tên banner đã tồn tại.");
                    return View(banner);
                }

                var success = await _bannerService.UpdateBannerAsync(banner);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật banner thành công!";
                    return RedirectToAction("Banners");
                }

                ModelState.AddModelError(string.Empty, "Cập nhật banner thất bại.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating banner: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật banner.");
            }

            return View(banner);
        }

        [HttpPost]
        [Route("Banners/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            try
            {
                var success = await _bannerService.DeleteBannerAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Xóa banner thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa banner.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting banner: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa banner.";
            }

            return RedirectToAction("Banners");
        }

        [HttpGet]
        [Route("Banners/Search")]
        public async Task<IActionResult> SearchBanners(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return RedirectToAction("Banners");
                }

                var banners = await _bannerService.SearchBannersAsync(searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View("Banners", banners);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching banners: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tìm kiếm banner.";
                return RedirectToAction("Banners");
            }
        }

        // Product Management
        [HttpGet]
        [Route("Products")]
        public async Task<IActionResult> Products()
        {
            try
            {
                var products = await _productService.GetAllProductsForAdminAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading products: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách sản phẩm.";
                return View(new List<Pet_Shop.Models.Entities.Product>());
            }
        }

        [HttpGet]
        [Route("Products/Create")]
        public async Task<IActionResult> CreateProduct()
        {
            try
            {
                var categories = await _productService.GetAllCategoriesAsync();
                var brands = await _productService.GetAllBrandsAsync();
                
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading create product form: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form tạo sản phẩm.";
                return RedirectToAction("Products");
            }
        }

        [HttpPost]
        [Route("Products/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Pet_Shop.Models.Entities.Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var categories = await _productService.GetAllCategoriesAsync();
                    var brands = await _productService.GetAllBrandsAsync();
                    
                    ViewBag.Categories = categories;
                    ViewBag.Brands = brands;
                    return View(product);
                }

                // Check if product name already exists
                var nameExists = await _productService.ProductExistsAsync(product.ProductName);
                if (nameExists)
                {
                    ModelState.AddModelError("ProductName", "Tên sản phẩm đã tồn tại.");
                    var categories = await _productService.GetAllCategoriesAsync();
                    var brands = await _productService.GetAllBrandsAsync();
                    
                    ViewBag.Categories = categories;
                    ViewBag.Brands = brands;
                    return View(product);
                }

                // Check if product code already exists (if provided)
                if (!string.IsNullOrEmpty(product.ProductCode))
                {
                    var codeExists = await _productService.ProductCodeExistsAsync(product.ProductCode);
                    if (codeExists)
                    {
                        ModelState.AddModelError("ProductCode", "Mã sản phẩm đã tồn tại.");
                        var categories = await _productService.GetAllCategoriesAsync();
                        var brands = await _productService.GetAllBrandsAsync();
                        
                        ViewBag.Categories = categories;
                        ViewBag.Brands = brands;
                        return View(product);
                    }
                }

                product.IsActive = true;
                product.CreatedDate = DateTime.Now;
                product.UpdatedDate = DateTime.Now;

                var success = await _productService.CreateProductAsync(product);
                if (success)
                {
                    // Handle product images if any
                    if (Request.Form.ContainsKey("ProductImages"))
                    {
                        var imageUrls = Request.Form["ProductImages"].ToString().Split(',');
                        for (int i = 0; i < imageUrls.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(imageUrls[i]))
                            {
                                await _productService.AddProductImageAsync(product.ProductID, imageUrls[i], i == 0);
                            }
                        }
                    }

                    TempData["SuccessMessage"] = "Tạo sản phẩm thành công!";
                    return RedirectToAction("Products");
                }

                ModelState.AddModelError(string.Empty, "Tạo sản phẩm thất bại.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating product: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi tạo sản phẩm.");
            }

            var categoriesForError = await _productService.GetAllCategoriesAsync();
            var brandsForError = await _productService.GetAllBrandsAsync();
            
            ViewBag.Categories = categoriesForError;
            ViewBag.Brands = brandsForError;
            return View(product);
        }

        [HttpGet]
        [Route("Products/Edit/{id}")]
        public async Task<IActionResult> EditProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                    return RedirectToAction("Products");
                }

                var categories = await _productService.GetAllCategoriesAsync();
                var brands = await _productService.GetAllBrandsAsync();
                
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading edit product form: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form chỉnh sửa sản phẩm.";
                return RedirectToAction("Products");
            }
        }

        [HttpPost]
        [Route("Products/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Pet_Shop.Models.Entities.Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var categories = await _productService.GetAllCategoriesAsync();
                    var brands = await _productService.GetAllBrandsAsync();
                    
                    ViewBag.Categories = categories;
                    ViewBag.Brands = brands;
                    return View(product);
                }

                // Check if product name already exists (excluding current product)
                var nameExists = await _productService.ProductExistsAsync(product.ProductName, product.ProductID);
                if (nameExists)
                {
                    ModelState.AddModelError("ProductName", "Tên sản phẩm đã tồn tại.");
                    var categories = await _productService.GetAllCategoriesAsync();
                    var brands = await _productService.GetAllBrandsAsync();
                    
                    ViewBag.Categories = categories;
                    ViewBag.Brands = brands;
                    return View(product);
                }

                // Check if product code already exists (if provided)
                if (!string.IsNullOrEmpty(product.ProductCode))
                {
                    var codeExists = await _productService.ProductCodeExistsAsync(product.ProductCode, product.ProductID);
                    if (codeExists)
                    {
                        ModelState.AddModelError("ProductCode", "Mã sản phẩm đã tồn tại.");
                        var categories = await _productService.GetAllCategoriesAsync();
                        var brands = await _productService.GetAllBrandsAsync();
                        
                        ViewBag.Categories = categories;
                        ViewBag.Brands = brands;
                        return View(product);
                    }
                }

                var success = await _productService.UpdateProductAsync(product);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction("Products");
                }

                ModelState.AddModelError(string.Empty, "Cập nhật sản phẩm thất bại.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating product: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật sản phẩm.");
            }

            var categoriesForError = await _productService.GetAllCategoriesAsync();
            var brandsForError = await _productService.GetAllBrandsAsync();
            
            ViewBag.Categories = categoriesForError;
            ViewBag.Brands = brandsForError;
            return View(product);
        }

        [HttpGet]
        [Route("Products/Details/{id}")]
        public async Task<IActionResult> ProductDetails(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                    return RedirectToAction("Products");
                }

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading product details: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết sản phẩm.";
                return RedirectToAction("Products");
            }
        }

        [HttpPost]
        [Route("Products/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var success = await _productService.DeleteProductAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa sản phẩm.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting product: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa sản phẩm.";
            }

            return RedirectToAction("Products");
        }

        [HttpGet]
        [Route("Products/Search")]
        public async Task<IActionResult> SearchProducts(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return RedirectToAction("Products");
                }

                var products = await _productService.SearchProductsAsync(searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View("Products", products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching products: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tìm kiếm sản phẩm.";
                return RedirectToAction("Products");
            }
        }

        // Product Image Management
        [HttpPost]
        [Route("Products/{productId}/Images/Add")]
        public async Task<IActionResult> AddProductImage(int productId, string imageUrl, bool isPrimary = false)
        {
            try
            {
                var success = await _productService.AddProductImageAsync(productId, imageUrl, isPrimary);
                if (success)
                {
                    return Json(new { success = true, message = "Thêm hình ảnh thành công!" });
                }
                return Json(new { success = false, message = "Thêm hình ảnh thất bại." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding product image: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm hình ảnh." });
            }
        }

        [HttpPost]
        [Route("Products/Images/{imageId}/Remove")]
        public async Task<IActionResult> RemoveProductImage(int imageId)
        {
            try
            {
                var success = await _productService.RemoveProductImageAsync(imageId);
                if (success)
                {
                    return Json(new { success = true, message = "Xóa hình ảnh thành công!" });
                }
                return Json(new { success = false, message = "Xóa hình ảnh thất bại." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing product image: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa hình ảnh." });
            }
        }

        [HttpPost]
        [Route("Products/{productId}/Images/{imageId}/SetPrimary")]
        public async Task<IActionResult> SetPrimaryImage(int productId, int imageId)
        {
            try
            {
                var success = await _productService.SetPrimaryImageAsync(productId, imageId);
                if (success)
                {
                    return Json(new { success = true, message = "Đặt hình ảnh chính thành công!" });
                }
                return Json(new { success = false, message = "Đặt hình ảnh chính thất bại." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting primary image: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi đặt hình ảnh chính." });
            }
        }

        [HttpGet]
        [Route("Products/{productId}/Images")]
        public async Task<IActionResult> GetProductImages(int productId)
        {
            try
            {
                var images = await _productService.GetProductImagesAsync(productId);
                return Json(new { success = true, images = images });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting product images: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi tải hình ảnh." });
            }
        }
    }
}
