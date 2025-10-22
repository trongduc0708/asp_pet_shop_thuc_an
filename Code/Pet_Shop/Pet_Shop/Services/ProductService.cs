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

        public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int categoryId, int count = 4)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Where(p => p.CategoryID == categoryId && p.ProductID != productId && p.IsActive)
                .OrderBy(p => Guid.NewGuid()) // Random order
                .Take(count)
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

        // CRUD Operations for Admin
        public async Task<IEnumerable<Product>> GetAllProductsForAdminAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> CreateProductAsync(Product product)
        {
            try
            {
                product.CreatedDate = DateTime.Now;
                product.UpdatedDate = DateTime.Now;
                
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                var existingProduct = await _context.Products.FindAsync(product.ProductID);
                if (existingProduct == null)
                    return false;

                existingProduct.ProductName = product.ProductName;
                existingProduct.ProductCode = product.ProductCode;
                existingProduct.CategoryID = product.CategoryID;
                existingProduct.BrandID = product.BrandID;
                existingProduct.ProductType = product.ProductType;
                existingProduct.PetType = product.PetType;
                existingProduct.Weight = product.Weight;
                existingProduct.Dimensions = product.Dimensions;
                existingProduct.ExpiryDate = product.ExpiryDate;
                existingProduct.Description = product.Description;
                existingProduct.ShortDescription = product.ShortDescription;
                existingProduct.Price = product.Price;
                existingProduct.SalePrice = product.SalePrice;
                existingProduct.Cost = product.Cost;
                existingProduct.IsNew = product.IsNew;
                existingProduct.IsActive = product.IsActive;
                existingProduct.IsFeatured = product.IsFeatured;
                existingProduct.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    return false;

                // Soft delete - set IsActive to false
                product.IsActive = false;
                product.UpdatedDate = DateTime.Now;
                
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ProductExistsAsync(string productName, int? excludeId = null)
        {
            var query = _context.Products.Where(p => p.ProductName == productName);
            
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.ProductID != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        public async Task<bool> ProductCodeExistsAsync(string productCode, int? excludeId = null)
        {
            if (string.IsNullOrEmpty(productCode))
                return false;
                
            var query = _context.Products.Where(p => p.ProductCode == productCode);
            
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.ProductID != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        // Product Image Management
        public async Task<bool> AddProductImageAsync(int productId, string imageUrl, bool isPrimary = false)
        {
            try
            {
                var productImage = new ProductImage
                {
                    ProductID = productId,
                    ImageURL = imageUrl,
                    IsPrimary = isPrimary,
                    CreatedDate = DateTime.Now
                };

                _context.ProductImages.Add(productImage);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveProductImageAsync(int imageId)
        {
            try
            {
                var image = await _context.ProductImages.FindAsync(imageId);
                if (image == null)
                    return false;

                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetPrimaryImageAsync(int productId, int imageId)
        {
            try
            {
                // Remove primary status from all images of this product
                var allImages = await _context.ProductImages
                    .Where(pi => pi.ProductID == productId)
                    .ToListAsync();

                foreach (var img in allImages)
                {
                    img.IsPrimary = false;
                }

                // Set the selected image as primary
                var primaryImage = allImages.FirstOrDefault(pi => pi.ImageID == imageId);
                if (primaryImage != null)
                {
                    primaryImage.IsPrimary = true;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<ProductImage>> GetProductImagesAsync(int productId)
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductID == productId)
                .OrderBy(pi => pi.IsPrimary ? 0 : 1)
                .ThenBy(pi => pi.CreatedDate)
                .ToListAsync();
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
