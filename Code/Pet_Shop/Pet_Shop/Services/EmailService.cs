using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Pet_Shop.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"];

                using var client = new SmtpClient(smtpServer, smtpPort);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                using var message = new MailMessage();
                message.From = new MailAddress(fromEmail, fromName);
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;

                await client.SendMailAsync(message);
                _logger.LogInformation($"Email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {toEmail}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName)
        {
            var subject = "Đặt lại mật khẩu - HyHy Pet Shop";
            var resetUrl = $"https://localhost:7000/Account/ResetPassword?token={resetToken}&email={Uri.EscapeDataString(toEmail)}";
            
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Đặt lại mật khẩu</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #d4af37; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background-color: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .button {{ display: inline-block; background-color: #d4af37; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 14px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🐕 HyHy Pet Shop</h1>
                            <p>Đặt lại mật khẩu của bạn</p>
                        </div>
                        <div class='content'>
                            <h2>Xin chào {userName}!</h2>
                            <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn tại HyHy Pet Shop.</p>
                            <p>Để đặt lại mật khẩu, vui lòng nhấn vào nút bên dưới:</p>
                            <p style='text-align: center;'>
                                <a href='{resetUrl}' class='button'>Đặt lại mật khẩu</a>
                            </p>
                            <p><strong>Lưu ý:</strong></p>
                            <ul>
                                <li>Link này chỉ có hiệu lực trong 24 giờ</li>
                                <li>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này</li>
                                <li>Để bảo mật, không chia sẻ link này với ai khác</li>
                            </ul>
                            <p>Nếu nút không hoạt động, bạn có thể copy và paste link sau vào trình duyệt:</p>
                            <p style='word-break: break-all; background-color: #e9ecef; padding: 10px; border-radius: 5px;'>
                                {resetUrl}
                            </p>
                        </div>
                        <div class='footer'>
                            <p>Trân trọng,<br>Đội ngũ HyHy Pet Shop</p>
                            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, body);
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Chào mừng đến với HyHy Pet Shop!";
            
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Chào mừng</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #d4af37; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background-color: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .button {{ display: inline-block; background-color: #d4af37; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 14px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🐕 HyHy Pet Shop</h1>
                            <p>Chào mừng bạn đến với gia đình!</p>
                        </div>
                        <div class='content'>
                            <h2>Xin chào {userName}!</h2>
                            <p>Cảm ơn bạn đã đăng ký tài khoản tại HyHy Pet Shop!</p>
                            <p>Bây giờ bạn có thể:</p>
                            <ul>
                                <li>🛒 Mua sắm hàng nghìn sản phẩm cho thú cưng</li>
                                <li>💝 Tích điểm và nhận ưu đãi đặc biệt</li>
                                <li>📦 Theo dõi đơn hàng dễ dàng</li>
                                <li>💬 Nhận tư vấn từ chuyên gia</li>
                            </ul>
                            <p style='text-align: center;'>
                                <a href='https://localhost:7000' class='button'>Bắt đầu mua sắm</a>
                            </p>
                            <p><strong>Ưu đãi đặc biệt cho thành viên mới:</strong></p>
                            <p>🎉 Giảm 10% cho đơn hàng đầu tiên!</p>
                            <p>🎁 Miễn phí vận chuyển cho đơn hàng từ 500.000 VNĐ</p>
                        </div>
                        <div class='footer'>
                            <p>Trân trọng,<br>Đội ngũ HyHy Pet Shop</p>
                            <p>Email: HyHy@Gmail.Com | Hotline: +980-34984089</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, body);
        }
    }
}
