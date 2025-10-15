using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;
using Pet_Shop.Models.ViewModels;
using System.Security.Claims;

namespace Pet_Shop.Services
{
    public class CartService
    {
        private readonly PetShopDbContext _context;
        private readonly ILogger<CartService> _logger;

        public CartService(PetShopDbContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CartItemViewModel>> GetCartItemsAsync(int userId)
        {
            try
            {
                var cartItems = await _context.Cart
                    .Include(c => c.Product)
                        .ThenInclude(p => p.Category)
                    .Include(c => c.Product)
                        .ThenInclude(p => p.Brand)
                    .Include(c => c.Product)
                        .ThenInclude(p => p.ProductImages)
                    .Include(c => c.Product)
                        .ThenInclude(p => p.Inventory)
                    .Where(c => c.UserID == userId)
                    .ToListAsync();

                return cartItems.Select(c => new CartItemViewModel
                {
                    ProductID = c.ProductID,
                    ProductName = c.Product.ProductName,
                    ProductImage = c.Product.ProductImages.FirstOrDefault()?.ImageURL ?? "/images/no-image.jpg",
                    Price = c.Product.Price,
                    SalePrice = c.Product.SalePrice,
                    Quantity = c.Quantity,
                    CategoryName = c.Product.Category.CategoryName,
                    BrandName = c.Product.Brand.BrandName,
                    IsAvailable = c.Product.IsActive,
                    StockQuantity = c.Product.Inventory?.QuantityInStock ?? 0
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting cart items for user {userId}: {ex.Message}");
                return new List<CartItemViewModel>();
            }
        }

        public async Task<bool> AddToCartAsync(int userId, int productId, int quantity = 1)
        {
            try
            {
                // Check if product exists and is active
                var product = await _context.Products
                    .Include(p => p.Inventory)
                    .FirstOrDefaultAsync(p => p.ProductID == productId && p.IsActive);

                if (product == null)
                {
                    _logger.LogWarning($"Product {productId} not found or inactive");
                    return false;
                }

                // Check stock availability
                if (product.Inventory != null && product.Inventory.QuantityInStock < quantity)
                {
                    _logger.LogWarning($"Insufficient stock for product {productId}. Available: {product.Inventory.QuantityInStock}, Requested: {quantity}");
                    return false;
                }

                // Check if item already exists in cart
                var existingCartItem = await _context.Cart
                    .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == productId);

                if (existingCartItem != null)
                {
                    // Update quantity
                    existingCartItem.Quantity += quantity;
                    
                    // Check stock again
                    if (product.Inventory != null && product.Inventory.QuantityInStock < existingCartItem.Quantity)
                    {
                        _logger.LogWarning($"Insufficient stock after update for product {productId}");
                        return false;
                    }
                }
                else
                {
                    // Add new item to cart
                    var cartItem = new Cart
                    {
                        UserID = userId,
                        ProductID = productId,
                        Quantity = quantity,
                        AddedDate = DateTime.Now
                    };
                    _context.Cart.Add(cartItem);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Added product {productId} to cart for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding product {productId} to cart for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateQuantityAsync(int userId, int productId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return await RemoveItemAsync(userId, productId);
                }

                var cartItem = await _context.Cart
                    .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == productId);

                if (cartItem == null)
                {
                    _logger.LogWarning($"Cart item not found for user {userId}, product {productId}");
                    return false;
                }

                // Check stock availability
                var product = await _context.Products
                    .Include(p => p.Inventory)
                    .FirstOrDefaultAsync(p => p.ProductID == productId);

                if (product?.Inventory != null && product.Inventory.QuantityInStock < quantity)
                {
                    _logger.LogWarning($"Insufficient stock for product {productId}. Available: {product.Inventory.QuantityInStock}, Requested: {quantity}");
                    return false;
                }

                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Updated quantity for product {productId} in cart for user {userId} to {quantity}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating quantity for product {productId} in cart for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveItemAsync(int userId, int productId)
        {
            try
            {
                var cartItem = await _context.Cart
                    .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == productId);

                if (cartItem == null)
                {
                    _logger.LogWarning($"Cart item not found for user {userId}, product {productId}");
                    return false;
                }

                _context.Cart.Remove(cartItem);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Removed product {productId} from cart for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing product {productId} from cart for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            try
            {
                var cartItems = await _context.Cart
                    .Where(c => c.UserID == userId)
                    .ToListAsync();

                _context.Cart.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Cleared cart for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error clearing cart for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<int> GetCartCountAsync(int userId)
        {
            try
            {
                return await _context.Cart
                    .Where(c => c.UserID == userId)
                    .SumAsync(c => c.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting cart count for user {userId}: {ex.Message}");
                return 0;
            }
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            try
            {
                var cartItems = await _context.Cart
                    .Include(c => c.Product)
                    .Where(c => c.UserID == userId)
                    .ToListAsync();

                return cartItems.Sum(c => 
                {
                    var price = c.Product.SalePrice ?? c.Product.Price;
                    return price * c.Quantity;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating cart total for user {userId}: {ex.Message}");
                return 0;
            }
        }
    }
}
