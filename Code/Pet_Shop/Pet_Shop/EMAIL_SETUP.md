# 📧 Hướng dẫn cấu hình Gmail cho HyHy Pet Shop

## 🔧 Cấu hình Gmail SMTP

### 1. Tạo App Password cho Gmail

1. **Đăng nhập vào Gmail** của bạn
2. **Vào Settings** → **Security** → **2-Step Verification** (bật nếu chưa có)
3. **Tạo App Password:**
   - Vào **Security** → **App passwords**
   - Chọn **Mail** và **Other (Custom name)**
   - Nhập tên: "HyHy Pet Shop"
   - Copy password được tạo (16 ký tự)

### 2. Cập nhật cấu hình

Mở file `appsettings.Development.json` và cập nhật:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-actual-email@gmail.com",
    "SmtpPassword": "your-16-character-app-password",
    "FromEmail": "HyHy@Gmail.Com",
    "FromName": "HyHy Pet Shop"
  }
}
```

### 3. Ví dụ cấu hình

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "hyhy.petshop@gmail.com",
    "SmtpPassword": "abcd efgh ijkl mnop",
    "FromEmail": "HyHy@Gmail.Com",
    "FromName": "HyHy Pet Shop"
  }
}
```

## 🚀 Test Email Service

### 1. Chạy ứng dụng
```bash
dotnet run
```

### 2. Test quên mật khẩu
1. Truy cập `/Account/ForgotPassword`
2. Nhập email hợp lệ
3. Kiểm tra hộp thư (có thể trong Spam)

### 3. Test đăng ký
1. Truy cập `/Account/Register`
2. Đăng ký tài khoản mới
3. Kiểm tra email chào mừng

## 📧 Email Templates

### 1. Forgot Password Email
- **Subject:** "Đặt lại mật khẩu - HyHy Pet Shop"
- **Content:** HTML template với link reset
- **Features:** Responsive design, branding

### 2. Welcome Email
- **Subject:** "Chào mừng đến với HyHy Pet Shop!"
- **Content:** HTML template với ưu đãi
- **Features:** Welcome message, benefits, CTA

## 🔒 Bảo mật

### 1. App Password
- ✅ Sử dụng App Password thay vì mật khẩu chính
- ✅ Không commit password vào Git
- ✅ Sử dụng User Secrets cho production

### 2. User Secrets (Production)
```bash
dotnet user-secrets set "EmailSettings:SmtpUsername" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPassword" "your-app-password"
```

## 🐛 Troubleshooting

### 1. Lỗi "Authentication failed"
- ✅ Kiểm tra App Password
- ✅ Đảm bảo 2-Step Verification đã bật
- ✅ Kiểm tra username (email đầy đủ)

### 2. Lỗi "Connection timeout"
- ✅ Kiểm tra firewall
- ✅ Thử port 465 với SSL
- ✅ Kiểm tra network connection

### 3. Email không gửi được
- ✅ Kiểm tra logs trong console
- ✅ Kiểm tra Spam folder
- ✅ Test với email khác

## 📝 Logs

Xem logs email trong console:
```
info: Pet_Shop.Services.EmailService[0]
      Email sent successfully to user@example.com
```

## 🎯 Production Setup

### 1. Azure App Service
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "@Microsoft.KeyVault(SecretUri=https://vault.vault.azure.net/secrets/email-username/)",
    "SmtpPassword": "@Microsoft.KeyVault(SecretUri=https://vault.vault.azure.net/secrets/email-password/)"
  }
}
```

### 2. Environment Variables
```bash
EmailSettings__SmtpUsername=your-email@gmail.com
EmailSettings__SmtpPassword=your-app-password
```

---

**Lưu ý:** Đảm bảo không commit thông tin email thật vào Git repository!
