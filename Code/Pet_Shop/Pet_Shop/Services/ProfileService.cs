using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;
using Pet_Shop.Models.Profile;
using System.Security.Cryptography;
using System.Text;

namespace Pet_Shop.Services
{
    public class ProfileService
    {
        private readonly PetShopDbContext _context;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(PetShopDbContext context, ILogger<ProfileService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProfileViewModel?> GetUserProfileAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.CustomerProfile)
                    .FirstOrDefaultAsync(u => u.UserID == userId);

                if (user == null)
                    return null;

                return new ProfileViewModel
                {
                    UserID = user.UserID,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    Address = user.Address,
                    CreatedDate = user.CreatedDate,
                    LastLoginDate = user.LastLoginDate,
                    RoleName = user.Role.RoleName,
                    DateOfBirth = user.CustomerProfile?.DateOfBirth,
                    Gender = user.CustomerProfile?.Gender,
                    TotalOrders = user.CustomerProfile?.TotalOrders ?? 0,
                    TotalSpent = user.CustomerProfile?.TotalSpent ?? 0m,
                    MembershipLevel = user.CustomerProfile?.MembershipLevel ?? "Bronze",
                    Points = user.CustomerProfile?.Points ?? 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user profile for ID {userId}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateProfileAsync(int userId, EditProfileViewModel model)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.CustomerProfile)
                    .FirstOrDefaultAsync(u => u.UserID == userId);

                if (user == null)
                    return false;

                // Update user info
                user.FullName = model.FullName;
                user.Phone = model.Phone;
                user.Address = model.Address;

                // Update or create customer profile
                if (user.CustomerProfile == null)
                {
                    user.CustomerProfile = new CustomerProfile
                    {
                        UserID = userId,
                        MembershipLevel = "Bronze",
                        Points = 0,
                        TotalOrders = 0,
                        TotalSpent = 0
                    };
                }

                user.CustomerProfile.DateOfBirth = model.DateOfBirth;
                user.CustomerProfile.Gender = model.Gender;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Profile updated for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating profile for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);
                if (user == null)
                    return false;

                // Verify current password
                if (!VerifyPassword(currentPassword, user.PasswordHash))
                    return false;

                // Update password
                user.PasswordHash = HashPassword(newPassword);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Password changed for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error changing password for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateMembershipLevelAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.CustomerProfile)
                    .FirstOrDefaultAsync(u => u.UserID == userId);

                if (user?.CustomerProfile == null)
                    return false;

                var totalSpent = user.CustomerProfile.TotalSpent;
                string newLevel;

                if (totalSpent >= 5000000) // 5 triệu
                    newLevel = "Platinum";
                else if (totalSpent >= 2000000) // 2 triệu
                    newLevel = "Gold";
                else if (totalSpent >= 500000) // 500k
                    newLevel = "Silver";
                else
                    newLevel = "Bronze";

                if (user.CustomerProfile.MembershipLevel != newLevel)
                {
                    user.CustomerProfile.MembershipLevel = newLevel;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Membership level updated to {newLevel} for user {userId}");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating membership level for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId, int page = 1, int pageSize = 10)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Status)
                    .Include(o => o.PaymentMethod)
                    .Where(o => o.UserID == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting orders for user {userId}: {ex.Message}");
                return new List<Order>();
            }
        }

        public async Task<int> GetUserOrdersCountAsync(int userId)
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.UserID == userId)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting orders count for user {userId}: {ex.Message}");
                return 0;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }
    }
}
