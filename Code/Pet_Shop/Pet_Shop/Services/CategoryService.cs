using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;

namespace Pet_Shop.Services
{
    public class CategoryService
    {
        private readonly PetShopDbContext _context;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(PetShopDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            try
            {
                return await _context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.SortOrder)
                    .ThenBy(c => c.CategoryName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all categories: {ex.Message}");
                return new List<Category>();
            }
        }

        public async Task<IEnumerable<Category>> GetParentCategoriesAsync()
        {
            try
            {
                return await _context.Categories
                    .Where(c => c.IsActive && c.ParentCategoryID == null)
                    .OrderBy(c => c.SortOrder)
                    .ThenBy(c => c.CategoryName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting parent categories: {ex.Message}");
                return new List<Category>();
            }
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId)
        {
            try
            {
                return await _context.Categories
                    .Where(c => c.IsActive && c.ParentCategoryID == parentCategoryId)
                    .OrderBy(c => c.SortOrder)
                    .ThenBy(c => c.CategoryName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting sub categories for parent {parentCategoryId}: {ex.Message}");
                return new List<Category>();
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.ParentCategory)
                    .Include(c => c.SubCategories)
                    .FirstOrDefaultAsync(c => c.CategoryID == categoryId && c.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting category by ID {categoryId}: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithProductCountAsync()
        {
            try
            {
                return await _context.Categories
                    .Where(c => c.IsActive)
                    .Include(c => c.Products.Where(p => p.IsActive))
                    .OrderBy(c => c.SortOrder)
                    .ThenBy(c => c.CategoryName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting categories with product count: {ex.Message}");
                return new List<Category>();
            }
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Category created: {category.CategoryName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating category: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            try
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Category updated: {category.CategoryName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating category: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category != null)
                {
                    category.IsActive = false;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Category deactivated: {category.CategoryName}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting category: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CategoryExistsAsync(string categoryName, int? parentCategoryId = null, int? excludeId = null)
        {
            try
            {
                var query = _context.Categories
                    .Where(c => c.CategoryName == categoryName && c.IsActive);

                if (parentCategoryId.HasValue)
                {
                    query = query.Where(c => c.ParentCategoryID == parentCategoryId);
                }
                else
                {
                    query = query.Where(c => c.ParentCategoryID == null);
                }

                if (excludeId.HasValue)
                {
                    query = query.Where(c => c.CategoryID != excludeId);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking category existence: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm)
        {
            try
            {
                return await _context.Categories
                    .Where(c => c.IsActive && 
                               (c.CategoryName.Contains(searchTerm) || 
                                c.Description!.Contains(searchTerm)))
                    .OrderBy(c => c.SortOrder)
                    .ThenBy(c => c.CategoryName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching categories: {ex.Message}");
                return new List<Category>();
            }
        }
    }
}
