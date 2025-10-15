using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;
using Pet_Shop.Models.ViewModels;

namespace Pet_Shop.Services
{
    public class WishlistService
    {
        private readonly PetShopDbContext _context;
        private readonly ILogger<WishlistService> _logger;

        public WishlistService(PetShopDbContext context, ILogger<WishlistService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<WishlistItemViewModel>> GetUserWishlistAsync(int userId)
        {
            try
            {
                var wishlistItems = await _context.Wishlist
                    .Include(w => w.Product)
                        .ThenInclude(p => p.Category)
                    .Include(w => w.Product)
                        .ThenInclude(p => p.Brand)
                    .Include(w => w.Product)
                        .ThenInclude(p => p.ProductImages)
                    .Include(w => w.Product)
                        .ThenInclude(p => p.Inventory)
                    .Where(w => w.UserID == userId)
                    .OrderByDescending(w => w.AddedDate)
                    .ToListAsync();

                return wishlistItems.Select(w => new WishlistItemViewModel
                {
                    WishlistID = w.WishlistID,
                    ProductID = w.ProductID,
                    ProductName = w.Product.ProductName,
                    ProductImage = w.Product.ProductImages.FirstOrDefault()?.ImageURL ?? "/images/no-image.jpg",
                    Price = w.Product.Price,
                    SalePrice = w.Product.SalePrice,
                    CategoryName = w.Product.Category.CategoryName,
                    BrandName = w.Product.Brand.BrandName,
                    IsAvailable = w.Product.IsActive,
                    StockQuantity = w.Product.Inventory?.QuantityInStock ?? 0,
                    AddedDate = w.AddedDate
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting wishlist for user {userId}: {ex.Message}");
                return new List<WishlistItemViewModel>();
            }
        }

        public async Task<bool> AddToWishlistAsync(int userId, int productId)
        {
            try
            {
                // Check if already in wishlist
                var existingItem = await _context.Wishlist
                    .FirstOrDefaultAsync(w => w.UserID == userId && w.ProductID == productId);

                if (existingItem != null)
                {
                    _logger.LogWarning($"Product {productId} already in wishlist for user {userId}");
                    return false;
                }

                // Check if product exists and is active
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductID == productId && p.IsActive);

                if (product == null)
                {
                    _logger.LogWarning($"Product {productId} not found or inactive");
                    return false;
                }

                var wishlistItem = new Wishlist
                {
                    UserID = userId,
                    ProductID = productId,
                    AddedDate = DateTime.Now
                };

                _context.Wishlist.Add(wishlistItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product {productId} added to wishlist for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding product {productId} to wishlist for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(int wishlistId, int userId)
        {
            try
            {
                var wishlistItem = await _context.Wishlist
                    .FirstOrDefaultAsync(w => w.WishlistID == wishlistId && w.UserID == userId);

                if (wishlistItem == null)
                {
                    _logger.LogWarning($"Wishlist item {wishlistId} not found for user {userId}");
                    return false;
                }

                _context.Wishlist.Remove(wishlistItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Wishlist item {wishlistId} removed for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing wishlist item {wishlistId} for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveFromWishlistByProductAsync(int userId, int productId)
        {
            try
            {
                var wishlistItem = await _context.Wishlist
                    .FirstOrDefaultAsync(w => w.UserID == userId && w.ProductID == productId);

                if (wishlistItem == null)
                {
                    _logger.LogWarning($"Product {productId} not found in wishlist for user {userId}");
                    return false;
                }

                _context.Wishlist.Remove(wishlistItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product {productId} removed from wishlist for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing product {productId} from wishlist for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsInWishlistAsync(int userId, int productId)
        {
            try
            {
                return await _context.Wishlist
                    .AnyAsync(w => w.UserID == userId && w.ProductID == productId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking if product {productId} is in wishlist for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<int> GetWishlistCountAsync(int userId)
        {
            try
            {
                return await _context.Wishlist
                    .CountAsync(w => w.UserID == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting wishlist count for user {userId}: {ex.Message}");
                return 0;
            }
        }
    }
}
