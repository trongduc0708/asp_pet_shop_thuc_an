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
    }
}
