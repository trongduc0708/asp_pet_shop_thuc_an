using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;

namespace Pet_Shop.Services
{
    public class BannerService
    {
        private readonly PetShopDbContext _context;
        private readonly ILogger<BannerService> _logger;

        public BannerService(PetShopDbContext context, ILogger<BannerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Banner>> GetAllBannersAsync()
        {
            try
            {
                return await _context.Banners
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.SortOrder)
                    .ThenBy(b => b.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all banners: {ex.Message}");
                return new List<Banner>();
            }
        }

        public async Task<IEnumerable<Banner>> GetActiveBannersAsync()
        {
            try
            {
                return await _context.Banners
                    .Where(b => b.IsActive && b.StartDate <= DateTime.Now && (b.EndDate == null || b.EndDate >= DateTime.Now))
                    .OrderBy(b => b.SortOrder)
                    .ThenBy(b => b.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting active banners: {ex.Message}");
                return new List<Banner>();
            }
        }

        public async Task<Banner?> GetBannerByIdAsync(int bannerId)
        {
            try
            {
                return await _context.Banners
                    .FirstOrDefaultAsync(b => b.BannerID == bannerId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting banner by ID {bannerId}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateBannerAsync(Banner banner)
        {
            try
            {
                _context.Banners.Add(banner);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Banner created: {banner.BannerName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating banner: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateBannerAsync(Banner banner)
        {
            try
            {
                _context.Banners.Update(banner);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Banner updated: {banner.BannerName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating banner: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteBannerAsync(int bannerId)
        {
            try
            {
                var banner = await _context.Banners.FindAsync(bannerId);
                if (banner != null)
                {
                    banner.IsActive = false;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Banner deactivated: {banner.BannerName}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting banner: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> BannerExistsAsync(string bannerName, int? excludeId = null)
        {
            try
            {
                var query = _context.Banners
                    .Where(b => b.BannerName == bannerName && b.IsActive);

                if (excludeId.HasValue)
                {
                    query = query.Where(b => b.BannerID != excludeId);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking banner existence: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Banner>> SearchBannersAsync(string searchTerm)
        {
            try
            {
                return await _context.Banners
                    .Where(b => b.IsActive && 
                               (b.BannerName.Contains(searchTerm) || 
                                b.AltText!.Contains(searchTerm)))
                    .OrderBy(b => b.SortOrder)
                    .ThenBy(b => b.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching banners: {ex.Message}");
                return new List<Banner>();
            }
        }

        public async Task<IEnumerable<Banner>> GetBannersByTypeAsync(string position)
        {
            try
            {
                return await _context.Banners
                    .Where(b => b.IsActive && b.Position == position)
                    .OrderBy(b => b.SortOrder)
                    .ThenBy(b => b.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting banners by position {position}: {ex.Message}");
                return new List<Banner>();
            }
        }
    }
}
