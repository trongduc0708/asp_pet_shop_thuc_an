using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Profile;
using Pet_Shop.Models.ViewModels;
using Pet_Shop.Services;
using System.Security.Claims;

namespace Pet_Shop.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ProfileService _profileService;
        private readonly AddressService _addressService;
        private readonly PetShopDbContext _context;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ProfileService profileService, AddressService addressService, PetShopDbContext context, ILogger<ProfileController> logger)
        {
            _profileService = profileService;
            _addressService = addressService;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                var profile = await _profileService.GetUserProfileAsync(userId);
                if (profile == null)
                {
                    TempData["ErrorMessage"] = "Không thể tải thông tin profile.";
                    return RedirectToAction("Index", "Home");
                }

                return View(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading profile: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải profile.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                var profile = await _profileService.GetUserProfileAsync(userId);
                if (profile == null)
                {
                    TempData["ErrorMessage"] = "Không thể tải thông tin profile.";
                    return RedirectToAction("Index");
                }

                var editModel = new EditProfileViewModel
                {
                    UserID = profile.UserID,
                    FullName = profile.FullName,
                    Phone = profile.Phone ?? string.Empty,
                    Address = profile.Address,
                    DateOfBirth = profile.DateOfBirth,
                    Gender = profile.Gender,
                    Username = profile.Username,
                    Email = profile.Email,
                    RoleName = profile.RoleName
                };

                return View(editModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading edit profile: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form chỉnh sửa.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                var success = await _profileService.UpdateProfileAsync(userId, model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "Cập nhật thông tin thất bại.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating profile: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật thông tin.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                var success = await _profileService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "Mật khẩu hiện tại không đúng.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error changing password: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi đổi mật khẩu.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Orders(int page = 1)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                const int pageSize = 10;
                var orders = await _profileService.GetUserOrdersAsync(userId, page, pageSize);
                var totalOrders = await _profileService.GetUserOrdersCountAsync(userId);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize);
                ViewBag.TotalOrders = totalOrders;

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading orders: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách đơn hàng.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                var order = await _context.Orders
                    .Include(o => o.Status)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderID == id && o.UserID == userId);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                    return RedirectToAction("Orders");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading order details: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết đơn hàng.";
                return RedirectToAction("Orders");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Addresses()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                var addresses = await _addressService.GetUserAddressesAsync(userId);
                return View(addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading addresses: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách địa chỉ.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult CreateAddress()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddress(AddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                var success = await _addressService.CreateAddressAsync(userId, model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Thêm địa chỉ thành công!";
                    return RedirectToAction("Addresses");
                }

                ModelState.AddModelError(string.Empty, "Thêm địa chỉ thất bại.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating address: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi thêm địa chỉ.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditAddress(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                var address = await _addressService.GetAddressByIdAsync(id, userId);
                if (address == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy địa chỉ.";
                    return RedirectToAction("Addresses");
                }

                return View(address);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading address: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải địa chỉ.";
                return RedirectToAction("Addresses");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(AddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return RedirectToAction("Login", "Account");
                }

                var success = await _addressService.UpdateAddressAsync(userId, model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cập nhật địa chỉ thành công!";
                    return RedirectToAction("Addresses");
                }

                ModelState.AddModelError(string.Empty, "Cập nhật địa chỉ thất bại.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating address: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật địa chỉ.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập." });
                }

                var success = await _addressService.DeleteAddressAsync(id, userId);
                if (success)
                {
                    return Json(new { success = true, message = "Xóa địa chỉ thành công!" });
                }

                return Json(new { success = false, message = "Xóa địa chỉ thất bại." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting address: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa địa chỉ." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập." });
                }

                var success = await _addressService.SetDefaultAddressAsync(id, userId);
                if (success)
                {
                    return Json(new { success = true, message = "Đặt địa chỉ mặc định thành công!" });
                }

                return Json(new { success = false, message = "Đặt địa chỉ mặc định thất bại." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting default address: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi đặt địa chỉ mặc định." });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
