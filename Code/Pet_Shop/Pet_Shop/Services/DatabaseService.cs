using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;

namespace Pet_Shop.Services
{
    public class DatabaseService
    {
        private readonly PetShopDbContext _context;

        public DatabaseService(PetShopDbContext context)
        {
            _context = context;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await _context.Database.CanConnectAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EnsureDatabaseCreatedAsync()
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database creation failed: {ex.Message}");
                return false;
            }
        }

        public async Task SeedInitialDataAsync()
        {
            try
            {
                // Check if data already exists
                if (await _context.UserRoles.AnyAsync())
                {
                    return; // Data already seeded
                }

                // Seed UserRoles
                var userRoles = new[]
                {
                    new Models.Entities.UserRole { RoleName = "Admin", Description = "Quản trị viên hệ thống" },
                    new Models.Entities.UserRole { RoleName = "Employee", Description = "Nhân viên cửa hàng" },
                    new Models.Entities.UserRole { RoleName = "Customer", Description = "Khách hàng" }
                };
                await _context.UserRoles.AddRangeAsync(userRoles);

                // Seed OrderStatuses
                var orderStatuses = new[]
                {
                    new Models.Entities.OrderStatus { StatusName = "New", Description = "Đơn hàng mới", SortOrder = 1 },
                    new Models.Entities.OrderStatus { StatusName = "Processing", Description = "Đang xử lý", SortOrder = 2 },
                    new Models.Entities.OrderStatus { StatusName = "Shipping", Description = "Đang giao hàng", SortOrder = 3 },
                    new Models.Entities.OrderStatus { StatusName = "Delivered", Description = "Đã giao hàng", SortOrder = 4 },
                    new Models.Entities.OrderStatus { StatusName = "Cancelled", Description = "Đã hủy", SortOrder = 5 }
                };
                await _context.OrderStatuses.AddRangeAsync(orderStatuses);

                // Seed PaymentMethods
                var paymentMethods = new[]
                {
                    new Models.Entities.PaymentMethod { MethodName = "COD", Description = "Thanh toán khi nhận hàng" },
                    new Models.Entities.PaymentMethod { MethodName = "VNPay", Description = "Thanh toán qua VNPay" }
                };
                await _context.PaymentMethods.AddRangeAsync(paymentMethods);

                // Seed Categories
                var categories = new[]
                {
                    new Models.Entities.Category { CategoryName = "Thức ăn chó", Description = "Các loại thức ăn dành cho chó" },
                    new Models.Entities.Category { CategoryName = "Thức ăn mèo", Description = "Các loại thức ăn dành cho mèo" },
                    new Models.Entities.Category { CategoryName = "Phụ kiện chó", Description = "Dây dắt, chuồng, đồ chơi cho chó" },
                    new Models.Entities.Category { CategoryName = "Phụ kiện mèo", Description = "Khay cát, vòng cổ, đồ chơi cho mèo" }
                };
                await _context.Categories.AddRangeAsync(categories);

                // Seed Brands
                var brands = new[]
                {
                    new Models.Entities.Brand { BrandName = "Royal Canin", Description = "Thương hiệu thức ăn cao cấp cho thú cưng" },
                    new Models.Entities.Brand { BrandName = "Whiskas", Description = "Thức ăn cho mèo" },
                    new Models.Entities.Brand { BrandName = "Pedigree", Description = "Thức ăn cho chó" },
                    new Models.Entities.Brand { BrandName = "Felix", Description = "Thức ăn ướt cho mèo" }
                };
                await _context.Brands.AddRangeAsync(brands);

                await _context.SaveChangesAsync();
                Console.WriteLine("Initial data seeded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding data: {ex.Message}");
                throw;
            }
        }
    }
}
