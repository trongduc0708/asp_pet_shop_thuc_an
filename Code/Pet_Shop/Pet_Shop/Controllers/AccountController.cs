using Microsoft.AspNetCore.Mvc;
using Pet_Shop.Models.Auth;
using Pet_Shop.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Pet_Shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly Pet_Shop.Services.AuthenticationService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(Pet_Shop.Services.AuthenticationService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _authService.LoginAsync(model.Email, model.Password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.RoleName),
                    new Claim("FullName", user.FullName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(24)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                _logger.LogInformation("User {Email} logged in.", user.Email);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.AgreeToTerms)
            {
                ModelState.AddModelError(string.Empty, "Bạn phải đồng ý với điều khoản sử dụng.");
                return View(model);
            }

            var success = await _authService.RegisterAsync(
                model.FullName, 
                model.Username, 
                model.Email, 
                model.Phone, 
                model.Password, 
                model.Address);

            if (success)
            {
                TempData["SuccessMessage"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Đăng ký thất bại. Email hoặc tên đăng nhập có thể đã tồn tại.");
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _authService.ForgotPasswordAsync(model.Email);
            if (success)
            {
                TempData["SuccessMessage"] = "Hướng dẫn đặt lại mật khẩu đã được gửi đến email của bạn.";
            }
            else
            {
                TempData["ErrorMessage"] = "Email không tồn tại trong hệ thống.";
            }

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string? token = null)
        {
            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _authService.ResetPasswordAsync(model.Email, model.Password);
            if (success)
            {
                TempData["SuccessMessage"] = "Mật khẩu đã được đặt lại thành công.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Đặt lại mật khẩu thất bại.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }
    }
}
