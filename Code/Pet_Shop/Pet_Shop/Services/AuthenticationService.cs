using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Pet_Shop.Services
{
    public class AuthenticationService
    {
        private readonly PetShopDbContext _context;
        private readonly EmailService _emailService;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(PetShopDbContext context, EmailService emailService, ILogger<AuthenticationService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.CustomerProfile)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null)
                return null;

            if (VerifyPassword(password, user.PasswordHash))
            {
                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return user;
            }

            return null;
        }

        public async Task<bool> RegisterAsync(string fullName, string username, string email, string phone, string password, string? address = null)
        {
            try
            {
                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Email == email || u.Username == username))
                {
                    return false;
                }

                // Get Customer role
                var customerRole = await _context.UserRoles.FirstOrDefaultAsync(r => r.RoleName == "Customer");
                if (customerRole == null)
                {
                    return false;
                }

                // Create new user
                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = HashPassword(password),
                    FullName = fullName,
                    Phone = phone,
                    Address = address,
                    RoleID = customerRole.RoleID,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create customer profile
                var customerProfile = new CustomerProfile
                {
                    UserID = user.UserID,
                    MembershipLevel = "Bronze",
                    Points = 0,
                    TotalOrders = 0,
                    TotalSpent = 0
                };

                _context.CustomerProfiles.Add(customerProfile);
                await _context.SaveChangesAsync();

                // Send welcome email (non-blocking)
                try
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.SendWelcomeEmailAsync(email, fullName);
                            _logger.LogInformation($"Welcome email sent successfully to {email}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to send welcome email to {email}: {ex.Message}");
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error starting welcome email task for {email}: {ex.Message}");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
                if (user == null)
                    return false;

                // Generate reset token
                var resetToken = GenerateResetToken();
                
                // Store reset token in user record (you might want to add a ResetToken field to User entity)
                // For now, we'll use a simple approach
                user.PasswordHash = $"RESET_TOKEN_{resetToken}_{DateTime.Now:yyyyMMddHHmmss}";
                await _context.SaveChangesAsync();

                // Send reset email
                var emailSent = await _emailService.SendPasswordResetEmailAsync(email, resetToken, user.FullName);
                
                if (emailSent)
                {
                    _logger.LogInformation($"Password reset email sent to {email}");
                    return true;
                }
                else
                {
                    _logger.LogError($"Failed to send password reset email to {email}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ForgotPasswordAsync: {ex.Message}");
                return false;
            }
        }

        private string GenerateResetToken()
        {
            return Guid.NewGuid().ToString("N")[..16]; // Generate 16-character token
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
                if (user == null)
                    return false;

                user.PasswordHash = HashPassword(newPassword);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
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
