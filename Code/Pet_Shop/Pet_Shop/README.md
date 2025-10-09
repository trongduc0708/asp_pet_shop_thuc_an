# HyHy Pet Shop - Website Bán Phụ Kiện Thú Cưng

## 🐕 Tổng quan
Website bán phụ kiện và thức ăn cho chó mèo được xây dựng bằng ASP.NET Core MVC với giao diện hiện đại và thân thiện.

## ✨ Tính năng đã hoàn thành

### 🎨 Giao diện người dùng
- **Header hiện đại** với logo, thanh tìm kiếm và thông tin liên hệ
- **Navigation bar** với menu đầy đủ và dropdown categories
- **Hero section** với thiết kế bắt mắt theo yêu cầu
- **Product cards** với hiệu ứng hover và animation
- **Responsive design** tối ưu cho mobile và desktop

### 🔐 Hệ thống Authentication
- **Đăng nhập** với email/password
- **Đăng ký** tài khoản mới với validation
- **Quên mật khẩu** và đặt lại mật khẩu
- **Session management** với Cookie Authentication
- **Phân quyền** Admin/Employee/Customer

### 🛍️ Chức năng Shop
- **Trang chủ** với hero section và sản phẩm nổi bật
- **Danh mục sản phẩm** theo loại thú cưng
- **Tìm kiếm sản phẩm** với thanh search
- **Giỏ hàng** (UI ready, chưa implement logic)
- **Liên hệ** với form gửi tin nhắn

## 🚀 Cài đặt và chạy

### 1. Yêu cầu hệ thống
- .NET 7.0 SDK
- SQL Server (LocalDB hoặc SQL Server)
- Visual Studio 2022 hoặc VS Code

### 2. Cài đặt packages
```bash
dotnet restore
```

### 3. Cấu hình database
Cập nhật connection string trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PetShopDB_10_2025;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

### 4. Chạy ứng dụng
```bash
dotnet run
```

### 5. Truy cập website
- Mở trình duyệt và truy cập: `https://localhost:7000`
- Test database: `/Database/TestConnection`
- Demo products: `/Demo/Products`

## 📁 Cấu trúc dự án

```
Pet_Shop/
├── Controllers/
│   ├── HomeController.cs          # Trang chủ, liên hệ
│   ├── AccountController.cs       # Authentication
│   ├── DatabaseController.cs     # Test database
│   └── DemoController.cs         # Demo LINQ queries
├── Models/
│   ├── Auth/                      # Authentication models
│   │   ├── LoginViewModel.cs
│   │   ├── RegisterViewModel.cs
│   │   └── ForgotPasswordViewModel.cs
│   └── Entities/                  # Database entities
├── Services/
│   ├── AuthenticationService.cs   # Xử lý đăng nhập/đăng ký
│   ├── DatabaseService.cs        # Quản lý database
│   └── ProductService.cs         # Quản lý sản phẩm
├── Views/
│   ├── Home/                      # Trang chủ, liên hệ
│   ├── Account/                   # Authentication views
│   └── Shared/
│       └── _Layout.cshtml        # Layout chính
├── wwwroot/
│   ├── css/site.css              # Custom styles
│   └── js/site.js                # Custom JavaScript
└── Data/
    └── PetShopDbContext.cs       # Entity Framework context
```

## 🎨 Giao diện

### Header Design
- Logo "WAGGY Pet Shop" với icon chó
- Thanh tìm kiếm ở giữa
- Thông tin liên hệ bên phải
- Navigation menu với dropdown categories

### Hero Section
- Background gradient đẹp mắt
- Text "Best Destination For Your Pets"
- Badge "SAVE 10-20% OFF"
- Button "SHOP NOW" với icon
- Hình ảnh chó với đồ chơi

### Product Cards
- Hover effects với transform
- Giá tiền nổi bật
- Button "Thêm vào giỏ"
- Responsive grid layout

## 🔧 Tính năng kỹ thuật

### Authentication
- Cookie-based authentication
- Password hashing với SHA256
- Session management
- Role-based authorization

### Database
- Entity Framework Core 7.0
- Code-first approach
- LINQ to Entities queries
- AutoMapper integration

### Frontend
- Bootstrap 5
- Font Awesome icons
- Custom CSS animations
- Responsive design
- JavaScript interactions

## 📱 Responsive Design

Website được tối ưu cho:
- **Desktop** (1200px+)
- **Tablet** (768px - 1199px)
- **Mobile** (< 768px)

## 🎯 Tính năng sắp tới

- [ ] Shopping cart functionality
- [ ] Order management system
- [ ] Payment integration (VNPay)
- [ ] Admin dashboard
- [ ] Product management
- [ ] Inventory management
- [ ] Email notifications
- [ ] Image upload
- [ ] Product reviews
- [ ] Wishlist functionality

## 🐛 Debug và Test

### Test Database Connection
Truy cập `/Database/TestConnection` để:
- Kiểm tra kết nối database
- Tạo database tự động
- Seed dữ liệu mẫu

### Demo LINQ Queries
Truy cập `/Demo/Products` để:
- Xem demo LINQ to Entities
- Test search functionality
- Xem thống kê sản phẩm

## 📞 Hỗ trợ

Nếu gặp vấn đề, vui lòng:
1. Kiểm tra connection string
2. Đảm bảo SQL Server đang chạy
3. Chạy `dotnet restore` để cài đặt packages
4. Kiểm tra logs trong console

## 📄 License

Dự án được phát triển cho mục đích học tập và demo.

---

**HyHy Pet Shop** - Nơi tốt nhất cho thú cưng của bạn! 🐕🐱