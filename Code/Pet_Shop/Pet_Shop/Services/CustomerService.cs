using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;

namespace Pet_Shop.Services
{
    public class CustomerService
    {
        private readonly PetShopDbContext _context;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(PetShopDbContext context, ILogger<CustomerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Lấy danh sách tất cả khách hàng
        public async Task<IEnumerable<User>> GetAllCustomersAsync()
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.CustomerProfile)
                    .Where(u => u.Role.RoleName == "Customer")
                    .OrderByDescending(u => u.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all customers: {ex.Message}");
                throw;
            }
        }

        // Lấy thông tin chi tiết khách hàng
        public async Task<User?> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.CustomerProfile)
                    .Include(u => u.Addresses)
                    .Include(u => u.Orders)
                        .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(u => u.UserID == customerId && u.Role.RoleName == "Customer");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting customer by ID {customerId}: {ex.Message}");
                throw;
            }
        }

        // Cập nhật thông tin khách hàng
        public async Task<bool> UpdateCustomerAsync(User customer)
        {
            try
            {
                var existingCustomer = await _context.Users.FindAsync(customer.UserID);
                if (existingCustomer == null)
                    return false;

                // Cập nhật thông tin cơ bản
                existingCustomer.FullName = customer.FullName;
                existingCustomer.Email = customer.Email;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Address = customer.Address;
                existingCustomer.IsActive = customer.IsActive;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating customer {customer.UserID}: {ex.Message}");
                return false;
            }
        }

        // Cập nhật CustomerProfile
        public async Task<bool> UpdateCustomerProfileAsync(int customerId, CustomerProfile profile)
        {
            try
            {
                var existingProfile = await _context.CustomerProfiles
                    .FirstOrDefaultAsync(cp => cp.UserID == customerId);

                if (existingProfile == null)
                {
                    // Tạo mới profile nếu chưa có
                    profile.UserID = customerId;
                    _context.CustomerProfiles.Add(profile);
                }
                else
                {
                    // Cập nhật profile hiện có
                    existingProfile.DateOfBirth = profile.DateOfBirth;
                    existingProfile.Gender = profile.Gender;
                    existingProfile.MembershipLevel = profile.MembershipLevel;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating customer profile for customer {customerId}: {ex.Message}");
                return false;
            }
        }

        // Tìm kiếm khách hàng
        public async Task<IEnumerable<User>> SearchCustomersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                    return await GetAllCustomersAsync();

                return await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.CustomerProfile)
                    .Where(u => u.Role.RoleName == "Customer" &&
                               (u.FullName.Contains(searchTerm) ||
                                u.Email.Contains(searchTerm) ||
                                u.Phone.Contains(searchTerm) ||
                                u.Username.Contains(searchTerm)))
                    .OrderByDescending(u => u.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching customers with term '{searchTerm}': {ex.Message}");
                throw;
            }
        }

        // Lọc khách hàng theo trạng thái
        public async Task<IEnumerable<User>> GetCustomersByStatusAsync(bool isActive)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.CustomerProfile)
                    .Where(u => u.Role.RoleName == "Customer" && u.IsActive == isActive)
                    .OrderByDescending(u => u.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting customers by status {isActive}: {ex.Message}");
                throw;
            }
        }

        // Lọc khách hàng theo hạng thành viên
        public async Task<IEnumerable<User>> GetCustomersByMembershipLevelAsync(string membershipLevel)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.CustomerProfile)
                    .Where(u => u.Role.RoleName == "Customer" && 
                               u.CustomerProfile != null && 
                               u.CustomerProfile.MembershipLevel == membershipLevel)
                    .OrderByDescending(u => u.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting customers by membership level '{membershipLevel}': {ex.Message}");
                throw;
            }
        }

        // Lấy thống kê khách hàng
        public async Task<object> GetCustomerStatisticsAsync()
        {
            try
            {
                var totalCustomers = await _context.Users
                    .CountAsync(u => u.Role.RoleName == "Customer");

                var activeCustomers = await _context.Users
                    .CountAsync(u => u.Role.RoleName == "Customer" && u.IsActive);

                var inactiveCustomers = totalCustomers - activeCustomers;

                var membershipStats = await _context.CustomerProfiles
                    .GroupBy(cp => cp.MembershipLevel)
                    .Select(g => new { Level = g.Key, Count = g.Count() })
                    .ToListAsync();

                var newCustomersThisMonth = await _context.Users
                    .CountAsync(u => u.Role.RoleName == "Customer" && 
                                   u.CreatedDate >= DateTime.Now.AddMonths(-1));

                return new
                {
                    TotalCustomers = totalCustomers,
                    ActiveCustomers = activeCustomers,
                    InactiveCustomers = inactiveCustomers,
                    MembershipStats = membershipStats,
                    NewCustomersThisMonth = newCustomersThisMonth
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting customer statistics: {ex.Message}");
                throw;
            }
        }

        // Lấy lịch sử đơn hàng của khách hàng
        public async Task<IEnumerable<Order>> GetCustomerOrderHistoryAsync(int customerId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.Status)
                    .Include(o => o.PaymentMethod)
                    .Where(o => o.UserID == customerId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting order history for customer {customerId}: {ex.Message}");
                throw;
            }
        }

        // Kiểm tra email đã tồn tại chưa (trừ khách hàng hiện tại)
        public async Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null)
        {
            try
            {
                var query = _context.Users.Where(u => u.Email == email);
                
                if (excludeCustomerId.HasValue)
                {
                    query = query.Where(u => u.UserID != excludeCustomerId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking email existence '{email}': {ex.Message}");
                return false;
            }
        }

        // Kiểm tra username đã tồn tại chưa (trừ khách hàng hiện tại)
        public async Task<bool> UsernameExistsAsync(string username, int? excludeCustomerId = null)
        {
            try
            {
                var query = _context.Users.Where(u => u.Username == username);
                
                if (excludeCustomerId.HasValue)
                {
                    query = query.Where(u => u.UserID != excludeCustomerId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking username existence '{username}': {ex.Message}");
                return false;
            }
        }

        // Lấy danh sách hạng thành viên
        public List<string> GetMembershipLevels()
        {
            return new List<string> { "Bronze", "Silver", "Gold", "Platinum" };
        }

        // Tính toán hạng thành viên dựa trên tổng chi tiêu
        public string CalculateMembershipLevel(decimal totalSpent)
        {
            if (totalSpent >= 10000000) // 10 triệu VND
                return "Platinum";
            else if (totalSpent >= 5000000) // 5 triệu VND
                return "Gold";
            else if (totalSpent >= 2000000) // 2 triệu VND
                return "Silver";
            else
                return "Bronze";
        }

        // Cập nhật hạng thành viên tự động
        public async Task<bool> UpdateMembershipLevelAsync(int customerId)
        {
            try
            {
                var customer = await _context.Users
                    .Include(u => u.CustomerProfile)
                    .FirstOrDefaultAsync(u => u.UserID == customerId);

                if (customer?.CustomerProfile == null)
                    return false;

                var newLevel = CalculateMembershipLevel(customer.CustomerProfile.TotalSpent);
                
                if (customer.CustomerProfile.MembershipLevel != newLevel)
                {
                    customer.CustomerProfile.MembershipLevel = newLevel;
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating membership level for customer {customerId}: {ex.Message}");
                return false;
            }
        }
    }
}
