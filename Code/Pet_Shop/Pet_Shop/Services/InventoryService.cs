using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;
using Pet_Shop.Models.ViewModels;

namespace Pet_Shop.Services
{
    public class InventoryService
    {
        private readonly PetShopDbContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(PetShopDbContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Lấy danh sách tồn kho với thông tin sản phẩm
        public async Task<IEnumerable<InventoryViewModel>> GetAllInventoryAsync()
        {
            try
            {
                var inventory = await _context.Inventory
                    .Include(i => i.Product)
                    .ThenInclude(p => p.Category)
                    .OrderBy(i => i.Product.ProductName)
                    .ToListAsync();

                return inventory.Select(i => new InventoryViewModel
                {
                    InventoryID = i.InventoryID,
                    ProductID = i.ProductID,
                    ProductName = i.Product.ProductName,
                    CategoryName = i.Product.Category?.CategoryName ?? "Không có danh mục",
                    QuantityInStock = i.QuantityInStock,
                    MinStockLevel = i.MinStockLevel,
                    MaxStockLevel = i.MaxStockLevel,
                    LastUpdated = i.LastUpdated,
                    IsLowStock = i.QuantityInStock <= i.MinStockLevel,
                    IsOutOfStock = i.QuantityInStock == 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting inventory: {ex.Message}");
                return new List<InventoryViewModel>();
            }
        }

        // Lấy thông tin tồn kho theo sản phẩm
        public async Task<InventoryViewModel?> GetInventoryByProductIdAsync(int productId)
        {
            try
            {
                var inventory = await _context.Inventory
                    .Include(i => i.Product)
                    .ThenInclude(p => p.Category)
                    .FirstOrDefaultAsync(i => i.ProductID == productId);

                if (inventory == null) return null;

                return new InventoryViewModel
                {
                    InventoryID = inventory.InventoryID,
                    ProductID = inventory.ProductID,
                    ProductName = inventory.Product.ProductName,
                    CategoryName = inventory.Product.Category?.CategoryName ?? "Không có danh mục",
                    QuantityInStock = inventory.QuantityInStock,
                    MinStockLevel = inventory.MinStockLevel,
                    MaxStockLevel = inventory.MaxStockLevel,
                    LastUpdated = inventory.LastUpdated,
                    IsLowStock = inventory.QuantityInStock <= inventory.MinStockLevel,
                    IsOutOfStock = inventory.QuantityInStock == 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting inventory for product {productId}: {ex.Message}");
                return null;
            }
        }

        // Cập nhật tồn kho
        public async Task<bool> UpdateInventoryAsync(int productId, int quantity, string transactionType, int createdBy, string? referenceNumber = null, string? notes = null, decimal? unitPrice = null)
        {
            try
            {
                var inventory = await _context.Inventory
                    .FirstOrDefaultAsync(i => i.ProductID == productId);

                if (inventory == null)
                {
                    _logger.LogWarning($"Inventory not found for product {productId}");
                    return false;
                }

                // Tính toán số lượng mới
                var adjustment = transactionType == "Import" ? quantity : -quantity;
                var newQuantity = inventory.QuantityInStock + adjustment;

                // Kiểm tra số lượng âm
                if (newQuantity < 0)
                {
                    _logger.LogWarning($"Insufficient stock for product {productId}. Current: {inventory.QuantityInStock}, Requested: {quantity}");
                    return false;
                }

                // Cập nhật tồn kho
                inventory.QuantityInStock = newQuantity;
                inventory.LastUpdated = DateTime.Now;

                // Tạo giao dịch
                var transaction = new InventoryTransaction
                {
                    ProductID = productId,
                    TransactionType = transactionType,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    TotalValue = unitPrice.HasValue ? unitPrice.Value * quantity : null,
                    ReferenceNumber = referenceNumber,
                    Notes = notes,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                };

                _context.InventoryTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Inventory updated for product {productId}. Type: {transactionType}, Quantity: {quantity}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating inventory for product {productId}: {ex.Message}");
                return false;
            }
        }

        // Lấy danh sách giao dịch xuất nhập kho
        public async Task<IEnumerable<InventoryTransactionViewModel>> GetInventoryTransactionsAsync(int? productId = null, string? transactionType = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.InventoryTransactions
                    .Include(t => t.Product)
                    .ThenInclude(p => p.Category)
                    .Include(t => t.CreatedByUser)
                    .AsQueryable();

                if (productId.HasValue)
                    query = query.Where(t => t.ProductID == productId.Value);

                if (!string.IsNullOrEmpty(transactionType))
                    query = query.Where(t => t.TransactionType == transactionType);

                if (fromDate.HasValue)
                    query = query.Where(t => t.CreatedDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(t => t.CreatedDate <= toDate.Value);

                var transactions = await query
                    .OrderByDescending(t => t.CreatedDate)
                    .ToListAsync();

                return transactions.Select(t => new InventoryTransactionViewModel
                {
                    TransactionID = t.TransactionID,
                    ProductID = t.ProductID,
                    ProductName = t.Product.ProductName,
                    CategoryName = t.Product.Category?.CategoryName ?? "Không có danh mục",
                    TransactionType = t.TransactionType,
                    TransactionTypeName = GetTransactionTypeName(t.TransactionType),
                    Quantity = t.Quantity,
                    UnitPrice = t.UnitPrice,
                    TotalValue = t.TotalValue,
                    ReferenceNumber = t.ReferenceNumber,
                    Notes = t.Notes,
                    CreatedBy = t.CreatedBy,
                    CreatedByName = t.CreatedByUser?.FullName ?? "Không xác định",
                    CreatedDate = t.CreatedDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting inventory transactions: {ex.Message}");
                return new List<InventoryTransactionViewModel>();
            }
        }

        // Lấy thống kê tồn kho
        public async Task<InventoryStatisticsViewModel> GetInventoryStatisticsAsync()
        {
            try
            {
                var inventory = await _context.Inventory
                    .Include(i => i.Product)
                    .ToListAsync();

                var totalProducts = inventory.Count;
                var totalStockValue = inventory.Sum(i => i.QuantityInStock * i.Product.Price);
                var lowStockProducts = inventory.Count(i => i.QuantityInStock <= i.MinStockLevel);
                var outOfStockProducts = inventory.Count(i => i.QuantityInStock == 0);
                var overStockProducts = inventory.Count(i => i.QuantityInStock > i.MaxStockLevel);

                // Lấy giao dịch trong 30 ngày qua
                var thirtyDaysAgo = DateTime.Now.AddDays(-30);
                var recentTransactions = await _context.InventoryTransactions
                    .Where(t => t.CreatedDate >= thirtyDaysAgo)
                    .ToListAsync();

                var totalImports = recentTransactions.Where(t => t.TransactionType == "Import").Sum(t => t.Quantity);
                var totalExports = recentTransactions.Where(t => t.TransactionType == "Export").Sum(t => t.Quantity);

                return new InventoryStatisticsViewModel
                {
                    TotalProducts = totalProducts,
                    TotalStockValue = totalStockValue,
                    LowStockProducts = lowStockProducts,
                    OutOfStockProducts = outOfStockProducts,
                    OverStockProducts = overStockProducts,
                    TotalImports30Days = totalImports,
                    TotalExports30Days = totalExports,
                    LastUpdated = inventory.Max(i => i.LastUpdated)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting inventory statistics: {ex.Message}");
                return new InventoryStatisticsViewModel();
            }
        }

        // Lấy sản phẩm tồn kho thấp
        public async Task<IEnumerable<InventoryViewModel>> GetLowStockProductsAsync()
        {
            try
            {
                var lowStockProducts = await _context.Inventory
                    .Include(i => i.Product)
                    .ThenInclude(p => p.Category)
                    .Where(i => i.QuantityInStock <= i.MinStockLevel)
                    .OrderBy(i => i.QuantityInStock)
                    .ToListAsync();

                return lowStockProducts.Select(i => new InventoryViewModel
                {
                    InventoryID = i.InventoryID,
                    ProductID = i.ProductID,
                    ProductName = i.Product.ProductName,
                    CategoryName = i.Product.Category?.CategoryName ?? "Không có danh mục",
                    QuantityInStock = i.QuantityInStock,
                    MinStockLevel = i.MinStockLevel,
                    MaxStockLevel = i.MaxStockLevel,
                    LastUpdated = i.LastUpdated,
                    IsLowStock = true,
                    IsOutOfStock = i.QuantityInStock == 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting low stock products: {ex.Message}");
                return new List<InventoryViewModel>();
            }
        }

        // Tạo mã phiếu tự động
        public string GenerateReferenceNumber(string transactionType)
        {
            var prefix = transactionType == "Import" ? "NK" : "XK";
            var date = DateTime.Now.ToString("yyyyMMdd");
            var time = DateTime.Now.ToString("HHmmss");
            return $"{prefix}{date}{time}";
        }

        // Cập nhật mức tồn kho tối thiểu và tối đa
        public async Task<bool> UpdateStockLevelsAsync(int productId, int minStockLevel, int maxStockLevel)
        {
            try
            {
                var inventory = await _context.Inventory
                    .FirstOrDefaultAsync(i => i.ProductID == productId);

                if (inventory == null)
                {
                    _logger.LogWarning($"Inventory not found for product {productId}");
                    return false;
                }

                inventory.MinStockLevel = minStockLevel;
                inventory.MaxStockLevel = maxStockLevel;
                inventory.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Stock levels updated for product {productId}. Min: {minStockLevel}, Max: {maxStockLevel}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating stock levels for product {productId}: {ex.Message}");
                return false;
            }
        }

        // Xuất báo cáo tồn kho
        public async Task<byte[]> ExportInventoryReportAsync()
        {
            try
            {
                var inventory = await GetAllInventoryAsync();
                var statistics = await GetInventoryStatisticsAsync();
                
                // Tạo Excel file (có thể sử dụng EPPlus hoặc ClosedXML)
                // Ở đây tôi sẽ tạo một file CSV đơn giản
                var csv = new System.Text.StringBuilder();
                
                // Header
                csv.AppendLine("BÁO CÁO TỒN KHO");
                csv.AppendLine($"Ngày xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm}");
                csv.AppendLine();
                
                // Statistics
                csv.AppendLine("THỐNG KÊ TỔNG QUAN");
                csv.AppendLine($"Tổng sản phẩm,{statistics.TotalProducts}");
                csv.AppendLine($"Tổng giá trị tồn kho,{statistics.TotalStockValue:N0} VNĐ");
                csv.AppendLine($"Sản phẩm tồn kho thấp,{statistics.LowStockProducts}");
                csv.AppendLine($"Sản phẩm hết hàng,{statistics.OutOfStockProducts}");
                csv.AppendLine($"Sản phẩm tồn kho cao,{statistics.OverStockProducts}");
                csv.AppendLine();
                
                // Inventory details
                csv.AppendLine("CHI TIẾT TỒN KHO");
                csv.AppendLine("Mã sản phẩm,Tên sản phẩm,Danh mục,Tồn kho hiện tại,Tồn kho tối thiểu,Tồn kho tối đa,Trạng thái,Cập nhật cuối");
                
                foreach (var item in inventory)
                {
                    var status = item.IsOutOfStock ? "Hết hàng" : 
                                item.IsLowStock ? "Tồn kho thấp" : "Bình thường";
                    
                    csv.AppendLine($"{item.ProductID},{item.ProductName},{item.CategoryName},{item.QuantityInStock},{item.MinStockLevel},{item.MaxStockLevel},{status},{item.LastUpdated:dd/MM/yyyy HH:mm}");
                }
                
                return System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting inventory report: {ex.Message}");
                throw;
            }
        }

        // Lấy tên loại giao dịch
        private string GetTransactionTypeName(string transactionType)
        {
            return transactionType switch
            {
                "Import" => "Nhập kho",
                "Export" => "Xuất kho",
                "Adjustment" => "Điều chỉnh",
                _ => transactionType
            };
        }
    }
}
