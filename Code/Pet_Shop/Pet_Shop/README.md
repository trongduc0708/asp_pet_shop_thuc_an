# Pet Shop Website - ASP.NET Core MVC

## Tổng quan
Website bán phụ kiện và thức ăn cho chó mèo được xây dựng bằng ASP.NET Core MVC với Entity Framework Core và LINQ to Entities.

## Công nghệ sử dụng
- **ASP.NET Core 7.0** - Web framework
- **Entity Framework Core 7.0** - ORM
- **SQL Server** - Database
- **LINQ to Entities** - Data querying
- **AutoMapper** - Object mapping
- **Bootstrap 5** - UI framework

## Cấu trúc dự án

### Models/Entities
- `UserRole`, `User`, `CustomerProfile` - Quản lý người dùng
- `Category`, `Brand`, `Product`, `ProductImage` - Quản lý sản phẩm
- `Inventory`, `InventoryTransaction` - Quản lý kho
- `Address`, `Cart` - Địa chỉ và giỏ hàng
- `Promotion`, `ProductPromotion` - Khuyến mãi
- `OrderStatus`, `PaymentMethod`, `Order`, `OrderItem`, `OrderStatusHistory` - Quản lý đơn hàng
- `Banner`, `ContactMessage` - Banner và liên hệ

### Services
- `DatabaseService` - Quản lý kết nối và seed data
- `ProductService` - Demo LINQ to Entities queries

### Controllers
- `HomeController` - Trang chủ
- `DatabaseController` - Test kết nối database
- `DemoController` - Demo LINQ to Entities

## Cài đặt và chạy

### 1. Cài đặt packages
```bash
dotnet restore
```

### 2. Cấu hình database
- Cập nhật connection string trong `appsettings.json`
- Đảm bảo SQL Server đang chạy

### 3. Chạy ứng dụng
```bash
dotnet run
```

### 4. Test database
- Truy cập `/Database/TestConnection` để test kết nối
- Database sẽ được tạo tự động với dữ liệu mẫu

### 5. Demo LINQ to Entities
- Truy cập `/Demo/Products` để xem demo
- Sử dụng chức năng search để test queries

## LINQ to Entities Examples

### Lấy tất cả sản phẩm
```csharp
var products = await _context.Products
    .Include(p => p.Category)
    .Include(p => p.Brand)
    .Where(p => p.IsActive)
    .OrderBy(p => p.ProductName)
    .ToListAsync();
```

### Tìm kiếm sản phẩm
```csharp
var products = await _context.Products
    .Include(p => p.Category)
    .Include(p => p.Brand)
    .Where(p => p.IsActive && 
               p.ProductName.Contains(searchTerm))
    .ToListAsync();
```

### Thống kê theo danh mục
```csharp
var stats = await _context.Products
    .Where(p => p.IsActive)
    .GroupBy(p => p.Category.CategoryName)
    .Select(g => new {
        CategoryName = g.Key,
        ProductCount = g.Count(),
        AveragePrice = g.Average(p => p.Price)
    })
    .ToListAsync();
```

## Database Schema
Database được thiết kế với các bảng chính:
- **User Management**: UserRoles, Users, CustomerProfiles
- **Product Management**: Categories, Brands, Products, ProductImages
- **Inventory**: Inventory, InventoryTransactions
- **Orders**: Orders, OrderItems, OrderStatuses, PaymentMethods
- **Promotions**: Promotions, ProductPromotions
- **Support**: Banners, ContactMessages

## Tính năng đã implement
- ✅ Entity Framework Core setup
- ✅ LINQ to Entities queries
- ✅ Database connection testing
- ✅ Data seeding
- ✅ Product management demo
- ✅ Search functionality
- ✅ Statistics and reporting

## Tính năng sắp tới
- 🔄 User authentication & authorization
- 🔄 Shopping cart functionality
- 🔄 Order management
- 🔄 Admin dashboard
- 🔄 Image upload
- 🔄 Payment integration

## Hướng dẫn phát triển

### Thêm Entity mới
1. Tạo class trong `Models/Entities/`
2. Thêm DbSet vào `PetShopDbContext`
3. Cấu hình relationships trong `OnModelCreating`

### Thêm Service mới
1. Tạo class trong `Services/`
2. Đăng ký trong `Program.cs`
3. Inject vào Controller

### Thêm LINQ queries
- Sử dụng `Include()` để load related data
- Sử dụng `Where()` để filter
- Sử dụng `OrderBy()` để sort
- Sử dụng `GroupBy()` cho aggregation

## Liên hệ
Dự án được phát triển cho mục đích học tập và demo.
