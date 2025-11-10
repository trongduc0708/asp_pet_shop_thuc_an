using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using System.Text;

namespace Pet_Shop.Scripts
{
    /// <summary>
    /// Script để export data từ SQL Server sang CSV format cho ML training
    /// Chạy script này khi cần retrain models với data mới
    /// </summary>
    public class ExportDataForML
    {
        private readonly PetShopDbContext _context;

        public ExportDataForML(PetShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Export user-item interactions (cho CVAE-CF model)
        /// Format: user_id,item_id,order_id,quantity,event_type,timestamp
        /// </summary>
        public async Task<string> ExportInteractionsAsync(string outputPath)
        {
            var interactions = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.StatusID != 5 && // Không tính đơn hủy
                             !string.IsNullOrEmpty(oi.Product.ProductCode))
                .Select(oi => new
                {
                    UserID = oi.Order.UserID,
                    ItemID = oi.Product.ProductCode!,
                    OrderID = oi.Order.OrderNumber,
                    Quantity = oi.Quantity,
                    EventType = "purchase", // Có thể mở rộng với view, add_to_cart
                    Timestamp = oi.Order.OrderDate
                })
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("user_id,item_id,order_id,quantity,event_type,timestamp");

            foreach (var interaction in interactions)
            {
                csv.AppendLine($"{interaction.UserID},{interaction.ItemID},{interaction.OrderID}," +
                              $"{interaction.Quantity},{interaction.EventType},{interaction.Timestamp:yyyy-MM-dd HH:mm:ss}");
            }

            await File.WriteAllTextAsync(outputPath, csv.ToString());
            return $"Exported {interactions.Count} interactions to {outputPath}";
        }

        /// <summary>
        /// Export product features (cho CVAE-CBF model)
        /// Format: item_id,product_name,category,brand,pet_type,price,description
        /// </summary>
        public async Task<string> ExportProductsAsync(string outputPath)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.IsActive == true && 
                           !string.IsNullOrEmpty(p.ProductCode))
                .Select(p => new
                {
                    ItemID = p.ProductCode!,
                    ProductName = p.ProductName,
                    Category = p.Category.CategoryName,
                    Brand = p.Brand.BrandName,
                    PetType = p.PetType,
                    Price = p.SalePrice ?? p.Price,
                    Description = p.Description ?? p.ShortDescription ?? ""
                })
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("item_id,product_name,category,brand,pet_type,price,description");

            foreach (var product in products)
            {
                // Escape commas và quotes trong description
                var description = product.Description
                    .Replace("\"", "\"\"")
                    .Replace("\n", " ")
                    .Replace("\r", " ");
                
                csv.AppendLine($"\"{product.ItemID}\",\"{product.ProductName}\"," +
                              $"\"{product.Category}\",\"{product.Brand}\"," +
                              $"\"{product.PetType}\",{product.Price},\"{description}\"");
            }

            await File.WriteAllTextAsync(outputPath, csv.ToString());
            return $"Exported {products.Count} products to {outputPath}";
        }

        /// <summary>
        /// Export cả hai files
        /// </summary>
        public async Task<(string interactionsResult, string productsResult)> ExportAllAsync(
            string interactionsPath, 
            string productsPath)
        {
            var interactionsResult = await ExportInteractionsAsync(interactionsPath);
            var productsResult = await ExportProductsAsync(productsPath);
            
            return (interactionsResult, productsResult);
        }
    }
}

