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
        private readonly OrderService _orderService;
        private readonly CustomerService _customerService;
        private readonly PromotionService _promotionService;
        private readonly InventoryService _inventoryService;
        private readonly VNPayService _vnpayService;

        public AdminController(ILogger<AdminController> logger, CategoryService categoryService, ProductService productService, BannerService bannerService, OrderService orderService, CustomerService customerService, PromotionService promotionService, InventoryService inventoryService, VNPayService vnpayService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _productService = productService;
            _bannerService = bannerService;
            _orderService = orderService;
            _customerService = customerService;
            _promotionService = promotionService;
            _inventoryService = inventoryService;
            _vnpayService = vnpayService;
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

        // Order Management
        [HttpGet]
        [Route("Orders")]
        public async Task<IActionResult> Orders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersForAdminAsync();
                var orderStatuses = await _orderService.GetAllOrderStatusesAsync();
                
                ViewBag.OrderStatuses = orderStatuses;
                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading orders: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách đơn hàng.";
                return View(new List<Pet_Shop.Models.Entities.Order>());
            }
        }

        [HttpGet]
        [Route("Orders/Details/{id}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdForAdminAsync(id);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                    return RedirectToAction("Orders");
                }

                var orderStatuses = await _orderService.GetAllOrderStatusesAsync();
                ViewBag.OrderStatuses = orderStatuses;
                
                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading order details: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết đơn hàng.";
                return RedirectToAction("Orders");
            }
        }

        [HttpPost]
        [Route("Orders/{id}/UpdateStatus")]
        public async Task<IActionResult> UpdateOrderStatus(int id, int newStatusId, string? notes = null)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
                var success = await _orderService.UpdateOrderStatusAsync(id, newStatusId, userId, notes);
                
                if (success)
                {
                    return Json(new { success = true, message = "Cập nhật trạng thái đơn hàng thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật trạng thái đơn hàng thất bại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating order status: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái đơn hàng." });
            }
        }

        [HttpPost]
        [Route("Orders/{id}/UpdateNotes")]
        public async Task<IActionResult> UpdateOrderNotes(int id, string adminNotes)
        {
            try
            {
                var success = await _orderService.UpdateOrderAdminNotesAsync(id, adminNotes);
                
                if (success)
                {
                    return Json(new { success = true, message = "Cập nhật ghi chú thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật ghi chú thất bại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating order notes: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật ghi chú." });
            }
        }

        [HttpGet]
        [Route("Orders/Search")]
        public async Task<IActionResult> SearchOrders(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return RedirectToAction("Orders");
                }

                var orders = await _orderService.SearchOrdersAsync(searchTerm);
                var orderStatuses = await _orderService.GetAllOrderStatusesAsync();
                
                ViewBag.SearchTerm = searchTerm;
                ViewBag.OrderStatuses = orderStatuses;
                return View("Orders", orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching orders: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tìm kiếm đơn hàng.";
                return RedirectToAction("Orders");
            }
        }

        [HttpGet]
        [Route("Orders/Filter")]
        public async Task<IActionResult> FilterOrders(int? statusId)
        {
            try
            {
                IEnumerable<Pet_Shop.Models.Entities.Order> orders;
                
                if (statusId.HasValue)
                {
                    orders = await _orderService.GetOrdersByStatusAsync(statusId.Value);
                }
                else
                {
                    orders = await _orderService.GetAllOrdersForAdminAsync();
                }

                var orderStatuses = await _orderService.GetAllOrderStatusesAsync();
                
                ViewBag.SelectedStatusId = statusId;
                ViewBag.OrderStatuses = orderStatuses;
                return View("Orders", orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error filtering orders: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi lọc đơn hàng.";
                return RedirectToAction("Orders");
            }
        }

        // Customer Management
        [HttpGet]
        [Route("Customers")]
        public async Task<IActionResult> Customers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                var membershipLevels = _customerService.GetMembershipLevels();
                
                ViewBag.MembershipLevels = membershipLevels;
                return View(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading customers: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách khách hàng.";
                return View(new List<Pet_Shop.Models.Entities.User>());
            }
        }

        [HttpGet]
        [Route("Customers/Details/{id}")]
        public async Task<IActionResult> CustomerDetails(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                    return RedirectToAction("Customers");
                }

                var orderHistory = await _customerService.GetCustomerOrderHistoryAsync(id);
                
                var viewModel = new Pet_Shop.Models.ViewModels.CustomerDetailsViewModel
                {
                    Customer = customer,
                    CustomerProfile = customer.CustomerProfile,
                    Addresses = customer.Addresses.ToList(),
                    RecentOrders = orderHistory.Take(10).ToList(),
                    Statistics = new Pet_Shop.Models.ViewModels.CustomerStatistics
                    {
                        TotalOrders = customer.CustomerProfile?.TotalOrders ?? 0,
                        TotalSpent = customer.CustomerProfile?.TotalSpent ?? 0m,
                        AverageOrderValue = customer.CustomerProfile?.TotalOrders > 0 
                            ? (customer.CustomerProfile?.TotalSpent ?? 0m) / (customer.CustomerProfile?.TotalOrders ?? 1) 
                            : 0m,
                        Points = customer.CustomerProfile?.Points ?? 0,
                        MembershipLevel = customer.CustomerProfile?.MembershipLevel ?? "Bronze",
                        LastOrderDate = orderHistory.FirstOrDefault()?.OrderDate,
                        DaysSinceLastOrder = orderHistory.FirstOrDefault()?.OrderDate != null 
                            ? (DateTime.Now - orderHistory.First().OrderDate).Days 
                            : 0
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading customer details for ID {id}: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết khách hàng.";
                return RedirectToAction("Customers");
            }
        }

        [HttpGet]
        [Route("Customers/Edit/{id}")]
        public async Task<IActionResult> EditCustomer(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy khách hàng.";
                    return RedirectToAction("Customers");
                }

                var membershipLevels = _customerService.GetMembershipLevels();
                var genders = new List<string> { "Nam", "Nữ", "Khác" };

                var viewModel = new Pet_Shop.Models.ViewModels.CustomerEditViewModel
                {
                    UserID = customer.UserID,
                    Username = customer.Username,
                    Email = customer.Email,
                    FullName = customer.FullName,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    IsActive = customer.IsActive,
                    DateOfBirth = customer.CustomerProfile?.DateOfBirth,
                    Gender = customer.CustomerProfile?.Gender,
                    MembershipLevel = customer.CustomerProfile?.MembershipLevel ?? "Bronze",
                    AvailableMembershipLevels = membershipLevels,
                    AvailableGenders = genders
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading edit customer form for ID {id}: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form chỉnh sửa khách hàng.";
                return RedirectToAction("Customers");
            }
        }

        [HttpPost]
        [Route("Customers/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(Pet_Shop.Models.ViewModels.CustomerEditViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var membershipLevels = _customerService.GetMembershipLevels();
                    var genders = new List<string> { "Nam", "Nữ", "Khác" };
                    
                    viewModel.AvailableMembershipLevels = membershipLevels;
                    viewModel.AvailableGenders = genders;
                    return View(viewModel);
                }

                // Kiểm tra email đã tồn tại chưa
                var emailExists = await _customerService.EmailExistsAsync(viewModel.Email, viewModel.UserID);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                    var membershipLevels = _customerService.GetMembershipLevels();
                    var genders = new List<string> { "Nam", "Nữ", "Khác" };
                    
                    viewModel.AvailableMembershipLevels = membershipLevels;
                    viewModel.AvailableGenders = genders;
                    return View(viewModel);
                }

                // Kiểm tra username đã tồn tại chưa
                var usernameExists = await _customerService.UsernameExistsAsync(viewModel.Username, viewModel.UserID);
                if (usernameExists)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                    var membershipLevels = _customerService.GetMembershipLevels();
                    var genders = new List<string> { "Nam", "Nữ", "Khác" };
                    
                    viewModel.AvailableMembershipLevels = membershipLevels;
                    viewModel.AvailableGenders = genders;
                    return View(viewModel);
                }

                // Cập nhật thông tin khách hàng
                var customer = new Pet_Shop.Models.Entities.User
                {
                    UserID = viewModel.UserID,
                    Username = viewModel.Username,
                    Email = viewModel.Email,
                    FullName = viewModel.FullName,
                    Phone = viewModel.Phone,
                    Address = viewModel.Address,
                    IsActive = viewModel.IsActive
                };

                var success = await _customerService.UpdateCustomerAsync(customer);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Cập nhật thông tin khách hàng thất bại.");
                    var membershipLevels = _customerService.GetMembershipLevels();
                    var genders = new List<string> { "Nam", "Nữ", "Khác" };
                    
                    viewModel.AvailableMembershipLevels = membershipLevels;
                    viewModel.AvailableGenders = genders;
                    return View(viewModel);
                }

                // Cập nhật CustomerProfile
                var profile = new Pet_Shop.Models.Entities.CustomerProfile
                {
                    DateOfBirth = viewModel.DateOfBirth,
                    Gender = viewModel.Gender,
                    MembershipLevel = viewModel.MembershipLevel
                };

                await _customerService.UpdateCustomerProfileAsync(viewModel.UserID, profile);

                TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng thành công!";
                return RedirectToAction("CustomerDetails", new { id = viewModel.UserID });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating customer {viewModel.UserID}: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật khách hàng.");
            }

            var membershipLevelsForError = _customerService.GetMembershipLevels();
            var gendersForError = new List<string> { "Nam", "Nữ", "Khác" };
            
            viewModel.AvailableMembershipLevels = membershipLevelsForError;
            viewModel.AvailableGenders = gendersForError;
            return View(viewModel);
        }

        [HttpGet]
        [Route("Customers/Search")]
        public async Task<IActionResult> SearchCustomers(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return RedirectToAction("Customers");
                }

                var customers = await _customerService.SearchCustomersAsync(searchTerm);
                var membershipLevels = _customerService.GetMembershipLevels();
                
                ViewBag.SearchTerm = searchTerm;
                ViewBag.MembershipLevels = membershipLevels;
                return View("Customers", customers);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching customers with term '{searchTerm}': {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tìm kiếm khách hàng.";
                return RedirectToAction("Customers");
            }
        }

        [HttpGet]
        [Route("Customers/Filter")]
        public async Task<IActionResult> FilterCustomers(bool? isActive, string? membershipLevel)
        {
            try
            {
                IEnumerable<Pet_Shop.Models.Entities.User> customers;
                
                if (isActive.HasValue)
                {
                    customers = await _customerService.GetCustomersByStatusAsync(isActive.Value);
                }
                else if (!string.IsNullOrEmpty(membershipLevel))
                {
                    customers = await _customerService.GetCustomersByMembershipLevelAsync(membershipLevel);
                }
                else
                {
                    customers = await _customerService.GetAllCustomersAsync();
                }

                var membershipLevels = _customerService.GetMembershipLevels();
                
                ViewBag.SelectedIsActive = isActive;
                ViewBag.SelectedMembershipLevel = membershipLevel;
                ViewBag.MembershipLevels = membershipLevels;
                return View("Customers", customers);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error filtering customers: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi lọc khách hàng.";
                return RedirectToAction("Customers");
            }
        }

        [HttpPost]
        [Route("Customers/{id}/ToggleStatus")]
        public async Task<IActionResult> ToggleCustomerStatus(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khách hàng." });
                }

                customer.IsActive = !customer.IsActive;
                var success = await _customerService.UpdateCustomerAsync(customer);
                
                if (success)
                {
                    var message = customer.IsActive ? "Kích hoạt khách hàng thành công!" : "Vô hiệu hóa khách hàng thành công!";
                    return Json(new { success = true, message = message, isActive = customer.IsActive });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật trạng thái khách hàng thất bại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error toggling customer status for ID {id}: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái khách hàng." });
            }
        }

        [HttpGet]
        [Route("Customers/Statistics")]
        public async Task<IActionResult> CustomerStatistics()
        {
            try
            {
                var statistics = await _customerService.GetCustomerStatisticsAsync();
                return Json(new { success = true, statistics = statistics });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting customer statistics: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi tải thống kê khách hàng." });
            }
        }

        // ========== PROMOTION MANAGEMENT ==========

        [HttpGet]
        [Route("Promotions")]
        public async Task<IActionResult> Promotions()
        {
            try
            {
                var promotions = await _promotionService.GetAllPromotionsForAdminAsync();
                return View(promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading promotions: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách mã khuyến mãi.";
                return View(new List<Pet_Shop.Models.Entities.Promotion>());
            }
        }

        [HttpGet]
        [Route("Promotions/Create")]
        public IActionResult CreatePromotion()
        {
            return View();
        }

        [HttpPost]
        [Route("Promotions/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePromotion(Pet_Shop.Models.Entities.Promotion promotion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(promotion);
                }

                // Check if promotion code already exists
                var exists = await _promotionService.PromotionCodeExistsAsync(promotion.PromotionCode);
                if (exists)
                {
                    ModelState.AddModelError("PromotionCode", "Mã khuyến mãi đã tồn tại.");
                    return View(promotion);
                }

                // Validate dates
                if (promotion.StartDate >= promotion.EndDate)
                {
                    ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu.");
                    return View(promotion);
                }

                promotion.IsActive = true;
                promotion.CreatedDate = DateTime.Now;

                var success = await _promotionService.CreatePromotionAsync(promotion);
                if (success)
                {
                    TempData["SuccessMessage"] = "Tạo mã khuyến mãi thành công!";
                    return RedirectToAction("Promotions");
                }

                ModelState.AddModelError(string.Empty, "Tạo mã khuyến mãi thất bại.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating promotion: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi tạo mã khuyến mãi.");
            }

            return View(promotion);
        }

        [HttpGet]
        [Route("Promotions/Edit/{id}")]
        public async Task<IActionResult> EditPromotion(int id)
        {
            try
            {
                var promotion = await _promotionService.GetPromotionByIdAsync(id);
                if (promotion == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy mã khuyến mãi.";
                    return RedirectToAction("Promotions");
                }

                return View(promotion);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading edit promotion form: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form chỉnh sửa mã khuyến mãi.";
                return RedirectToAction("Promotions");
            }
        }

        [HttpPost]
        [Route("Promotions/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotion(Pet_Shop.Models.Entities.Promotion promotion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(promotion);
                }

                // Check if promotion code already exists (excluding current promotion)
                var exists = await _promotionService.PromotionCodeExistsAsync(promotion.PromotionCode, promotion.PromotionID);
                if (exists)
                {
                    ModelState.AddModelError("PromotionCode", "Mã khuyến mãi đã tồn tại.");
                    return View(promotion);
                }

                // Validate dates
                if (promotion.StartDate >= promotion.EndDate)
                {
                    ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu.");
                    return View(promotion);
                }

                var success = await _promotionService.UpdatePromotionAsync(promotion);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật mã khuyến mãi thành công!";
                    return RedirectToAction("Promotions");
                }

                ModelState.AddModelError(string.Empty, "Cập nhật mã khuyến mãi thất bại.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating promotion: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật mã khuyến mãi.");
            }

            return View(promotion);
        }

        [HttpPost]
        [Route("Promotions/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            try
            {
                var success = await _promotionService.DeletePromotionAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Xóa mã khuyến mãi thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa mã khuyến mãi.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting promotion: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa mã khuyến mãi.";
            }

            return RedirectToAction("Promotions");
        }

        [HttpGet]
        [Route("Promotions/Search")]
        public async Task<IActionResult> SearchPromotions(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return RedirectToAction("Promotions");
                }

                var promotions = await _promotionService.SearchPromotionsAsync(searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View("Promotions", promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching promotions: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tìm kiếm mã khuyến mãi.";
                return RedirectToAction("Promotions");
            }
        }

        [HttpGet]
        [Route("Promotions/Filter")]
        public async Task<IActionResult> FilterPromotions(bool? isActive)
        {
            try
            {
                IEnumerable<Pet_Shop.Models.Entities.Promotion> promotions;
                
                if (isActive.HasValue)
                {
                    promotions = await _promotionService.GetPromotionsByStatusAsync(isActive.Value);
                }
                else
                {
                    promotions = await _promotionService.GetAllPromotionsForAdminAsync();
                }

                ViewBag.SelectedIsActive = isActive;
                return View("Promotions", promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error filtering promotions: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi lọc mã khuyến mãi.";
                return RedirectToAction("Promotions");
            }
        }

        [HttpPost]
        [Route("Promotions/{id}/ToggleStatus")]
        public async Task<IActionResult> TogglePromotionStatus(int id)
        {
            try
            {
                var promotion = await _promotionService.GetPromotionByIdAsync(id);
                if (promotion == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy mã khuyến mãi." });
                }

                promotion.IsActive = !promotion.IsActive;
                var success = await _promotionService.UpdatePromotionAsync(promotion);
                
                if (success)
                {
                    var message = promotion.IsActive ? "Kích hoạt mã khuyến mãi thành công!" : "Vô hiệu hóa mã khuyến mãi thành công!";
                    return Json(new { success = true, message = message, isActive = promotion.IsActive });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật trạng thái mã khuyến mãi thất bại." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error toggling promotion status for ID {id}: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái mã khuyến mãi." });
            }
        }

        [HttpGet]
        [Route("Promotions/Statistics")]
        public async Task<IActionResult> PromotionStatistics()
        {
            try
            {
                var statistics = await _promotionService.GetPromotionStatisticsAsync();
                return Json(new { success = true, statistics = statistics });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting promotion statistics: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi tải thống kê mã khuyến mãi." });
            }
        }

        #region Inventory Management

        [HttpGet]
        [Route("Inventory")]
        public async Task<IActionResult> Inventory()
        {
            try
            {
                var inventory = await _inventoryService.GetAllInventoryAsync();
                var statistics = await _inventoryService.GetInventoryStatisticsAsync();
                
                ViewBag.Statistics = statistics;
                return View(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading inventory: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách tồn kho.";
                return View(new List<Pet_Shop.Models.ViewModels.InventoryViewModel>());
            }
        }

        [HttpGet]
        [Route("InventoryTransactions")]
        public async Task<IActionResult> InventoryTransactions(int? productId, string? transactionType, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var transactions = await _inventoryService.GetInventoryTransactionsAsync(productId, transactionType, fromDate, toDate);
                var products = await _productService.GetAllProductsAsync();
                
                ViewBag.Products = products;
                ViewBag.SelectedProductId = productId;
                ViewBag.SelectedTransactionType = transactionType;
                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
                
                return View(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading inventory transactions: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải lịch sử giao dịch.";
                return View(new List<Pet_Shop.Models.ViewModels.InventoryTransactionViewModel>());
            }
        }

        [HttpGet]
        [Route("CreateInventoryTransaction")]
        public async Task<IActionResult> CreateInventoryTransaction()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                ViewBag.Products = products;
                
                var model = new Pet_Shop.Models.ViewModels.InventoryTransactionFormViewModel();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading create inventory transaction form: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form tạo giao dịch.";
                return RedirectToAction("Inventory");
            }
        }

        [HttpPost]
        [Route("CreateInventoryTransaction")]
        public async Task<IActionResult> CreateInventoryTransaction(Pet_Shop.Models.ViewModels.InventoryTransactionFormViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var products = await _productService.GetAllProductsAsync();
                    ViewBag.Products = products;
                    return View(model);
                }

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                
                // Tạo mã phiếu tự động nếu không có
                if (string.IsNullOrEmpty(model.ReferenceNumber))
                {
                    model.ReferenceNumber = _inventoryService.GenerateReferenceNumber(model.TransactionType);
                }

                var success = await _inventoryService.UpdateInventoryAsync(
                    model.ProductID, 
                    model.Quantity, 
                    model.TransactionType, 
                    userId, 
                    model.ReferenceNumber, 
                    model.Notes, 
                    model.UnitPrice
                );

                if (success)
                {
                    TempData["SuccessMessage"] = "Tạo giao dịch xuất nhập kho thành công!";
                    return RedirectToAction("InventoryTransactions");
                }
                else
                {
                    TempData["ErrorMessage"] = "Tạo giao dịch thất bại. Vui lòng kiểm tra lại thông tin.";
                    var products = await _productService.GetAllProductsAsync();
                    ViewBag.Products = products;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating inventory transaction: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo giao dịch.";
                var products = await _productService.GetAllProductsAsync();
                ViewBag.Products = products;
                return View(model);
            }
        }

        [HttpGet]
        [Route("InventoryReports")]
        public async Task<IActionResult> InventoryReports()
        {
            try
            {
                var statistics = await _inventoryService.GetInventoryStatisticsAsync();
                var lowStockProducts = await _inventoryService.GetLowStockProductsAsync();
                
                ViewBag.LowStockProducts = lowStockProducts;
                return View(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading inventory reports: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải báo cáo tồn kho.";
                return View(new Pet_Shop.Models.ViewModels.InventoryStatisticsViewModel());
            }
        }

        [HttpGet]
        [Route("GetProductInventory")]
        public async Task<IActionResult> GetProductInventory(int productId)
        {
            try
            {
                var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);
                if (inventory == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin tồn kho." });
                }

                return Json(new { 
                    success = true, 
                    data = new {
                        productId = inventory.ProductID,
                        productName = inventory.ProductName,
                        currentStock = inventory.QuantityInStock,
                        minStockLevel = inventory.MinStockLevel,
                        maxStockLevel = inventory.MaxStockLevel,
                        isLowStock = inventory.IsLowStock,
                        isOutOfStock = inventory.IsOutOfStock
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting product inventory: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy thông tin tồn kho." });
            }
        }

        [HttpPost]
        [Route("UpdateStockLevels")]
        public async Task<IActionResult> UpdateStockLevels(int productId, int minStockLevel, int maxStockLevel)
        {
            try
            {
                if (minStockLevel < 0 || maxStockLevel < 0 || minStockLevel > maxStockLevel)
                {
                    return Json(new { success = false, message = "Mức tồn kho không hợp lệ." });
                }

                var success = await _inventoryService.UpdateStockLevelsAsync(productId, minStockLevel, maxStockLevel);
                
                if (success)
                {
                    return Json(new { success = true, message = "Cập nhật mức tồn kho thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật mức tồn kho." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating stock levels: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật mức tồn kho." });
            }
        }

        [HttpGet]
        [Route("ExportInventoryReport")]
        public async Task<IActionResult> ExportInventoryReport()
        {
            try
            {
                var reportData = await _inventoryService.ExportInventoryReportAsync();
                var fileName = $"BaoCaoTonKho_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                return File(reportData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting inventory report: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xuất báo cáo.";
                return RedirectToAction("InventoryReports");
            }
        }

        [HttpGet]
        [Route("TestVNPaySignature")]
        public IActionResult TestVNPaySignature()
        {
            try
            {
                var testOrderId = "TEST_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var testAmount = 100000m; // 100,000 VND
                var testReturnUrl = Url.Action("PaymentReturn", "Checkout", new { orderNumber = testOrderId }, Request.Scheme) ?? "";
                
                var debugInfo = _vnpayService.DebugSignatureGeneration(testOrderId, testAmount, testReturnUrl);
                
                return Content(debugInfo, "text/plain");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error testing VNPay signature: {ex.Message}");
                return Content($"Error: {ex.Message}", "text/plain");
            }
        }

        #endregion
    }
}
