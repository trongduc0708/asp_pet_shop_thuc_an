using Microsoft.AspNetCore.Mvc;
using Pet_Shop.Models.Entities;
using Pet_Shop.Services;
using System.Security.Claims;

namespace Pet_Shop.Controllers
{
    public class PolicyController : Controller
    {
        private readonly ILogger<PolicyController> _logger;
        private readonly EmailService _emailService;

        public PolicyController(ILogger<PolicyController> logger, EmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult About()
        {
            ViewData["Title"] = "Về chúng tôi";
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            ViewData["Title"] = "Liên hệ";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactMessage contactMessage)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(contactMessage);
                }

                // Set default values
                contactMessage.CreatedDate = DateTime.Now;
                contactMessage.Status = "New";

                // Here you would save to database
                // For now, we'll just log it
                _logger.LogInformation($"Contact message from {contactMessage.FullName} ({contactMessage.Email}): {contactMessage.Message}");

                TempData["SuccessMessage"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi trong thời gian sớm nhất.";
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing contact message: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi gửi tin nhắn. Vui lòng thử lại sau.");
                return View(contactMessage);
            }
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            ViewData["Title"] = "Chính sách bảo mật";
            return View();
        }

        [HttpGet]
        public IActionResult Terms()
        {
            ViewData["Title"] = "Điều khoản sử dụng";
            return View();
        }

        [HttpGet]
        public IActionResult Shipping()
        {
            ViewData["Title"] = "Chính sách vận chuyển";
            return View();
        }

        [HttpGet]
        public IActionResult Returns()
        {
            ViewData["Title"] = "Chính sách đổi trả";
            return View();
        }

        [HttpGet]
        public IActionResult FAQ()
        {
            ViewData["Title"] = "Câu hỏi thường gặp";
            return View();
        }

        [HttpGet]
        public IActionResult Sitemap()
        {
            ViewData["Title"] = "Sơ đồ trang web";
            return View();
        }
    }
}
