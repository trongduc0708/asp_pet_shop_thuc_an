using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;

namespace Pet_Shop.Services
{
    public class ProductService
    {
        private readonly PetShopDbContext _context;

        public ProductService(PetShopDbContext context)
        {
            _context = context;
        }

        // Demo LINQ to Entities queries
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductID == id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.CategoryID == categoryId && p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.BrandID == brandId && p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.IsActive && 
                           (p.ProductName.Contains(searchTerm) || 
                            p.Description!.Contains(searchTerm) ||
                            p.Category.CategoryName.Contains(searchTerm) ||
                            p.Brand.BrandName.Contains(searchTerm)))
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Where(p => p.IsFeatured && p.IsActive)
                .OrderBy(p => p.CreatedDate)
                .Take(10)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetNewProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Where(p => p.IsNew && p.IsActive)
                .OrderByDescending(p => p.CreatedDate)
                .Take(10)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.IsActive && p.Price >= minPrice && p.Price <= maxPrice)
                .OrderBy(p => p.Price)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
        {
            return await _context.Brands
                .Where(b => b.IsActive)
                .OrderBy(b => b.BrandName)
                .ToListAsync();
        }

        // Complex LINQ query example
        public async Task<object> GetProductStatisticsAsync()
        {
            var stats = await _context.Products
                .Where(p => p.IsActive)
                .GroupBy(p => p.Category.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    ProductCount = g.Count(),
                    AveragePrice = g.Average(p => p.Price),
                    MinPrice = g.Min(p => p.Price),
                    MaxPrice = g.Max(p => p.Price),
                    TotalValue = g.Sum(p => p.Price)
                })
                .OrderByDescending(s => s.ProductCount)
                .ToListAsync();

            return stats;
        }

        // Example of using stored procedures with Entity Framework
        public async Task<bool> UpdateInventoryAsync(int productId, int quantity, string transactionType, int createdBy)
        {
            try
            {
                // This would call the stored procedure sp_UpdateInventory
                // For now, we'll do it manually with LINQ
                var inventory = await _context.Inventory
                    .FirstOrDefaultAsync(i => i.ProductID == productId);

                if (inventory != null)
                {
                    var adjustment = transactionType == "Import" ? quantity : -quantity;
                    inventory.QuantityInStock += adjustment;
                    inventory.LastUpdated = DateTime.Now;

                    // Add transaction record
                    var transaction = new InventoryTransaction
                    {
                        ProductID = productId,
                        TransactionType = transactionType,
                        Quantity = quantity,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now
                    };

                    _context.InventoryTransactions.Add(transaction);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
