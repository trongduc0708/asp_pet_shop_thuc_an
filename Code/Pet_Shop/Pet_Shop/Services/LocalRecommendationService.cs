using System.Text.Json;
using System.Text;
using Pet_Shop.Models.Entities;
using Pet_Shop.Data;
using Microsoft.EntityFrameworkCore;

namespace Pet_Shop.Services
{
    /// <summary>
    /// Service để gọi Local Recommendation API (Python models đã train)
    /// Thay thế OpenAI embeddings bằng models CVAE-CF + CVAE-CBF + Hybrid
    /// </summary>
    public class LocalRecommendationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly PetShopDbContext _context;
        private readonly ILogger<LocalRecommendationService>? _logger;

        public LocalRecommendationService(
            HttpClient httpClient, 
            IConfiguration configuration,
            PetShopDbContext context,
            ILogger<LocalRecommendationService>? logger = null)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lấy API base URL từ config
        /// </summary>
        private string GetApiBaseUrl()
        {
            return _configuration["LocalMLSettings:ApiBaseUrl"] ?? "http://localhost:8000";
        }

        /// <summary>
        /// Kiểm tra API có sẵn sàng không
        /// </summary>
        public async Task<bool> IsApiAvailableAsync()
        {
            try
            {
                var baseUrl = GetApiBaseUrl();
                var response = await _httpClient.GetAsync($"{baseUrl}/health");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"Local Recommendation API not available: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy recommendations cho user sử dụng Hybrid model
        /// </summary>
        public async Task<List<Product>> GetRecommendedProductsAsync(int userId, int count = 10, int? currentProductId = null)
        {
            try
            {
                // Kiểm tra API có sẵn sàng không
                if (!await IsApiAvailableAsync())
                {
                    _logger?.LogWarning("Local Recommendation API not available, returning empty list");
                    return new List<Product>();
                }

                var baseUrl = GetApiBaseUrl();

                // Map ProductID sang item_id format (cần mapping từ database)
                string? currentItemId = null;
                if (currentProductId.HasValue)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.ProductID == currentProductId.Value);
                    if (product != null && !string.IsNullOrEmpty(product.ProductCode))
                    {
                        currentItemId = product.ProductCode;
                    }
                }

                // Tạo request body
                var requestBody = new
                {
                    user_id = userId,
                    current_item_id = currentItemId,
                    k = count
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Gọi API
                var response = await _httpClient.PostAsync($"{baseUrl}/recommend", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<RecommendationApiResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse?.Success == true && apiResponse.Recommendations != null)
                    {
                        // Map item_id về ProductID
                        var itemIds = apiResponse.Recommendations.Select(r => r.ItemId).ToList();
                        
                        // Tìm products theo ProductCode (item_id)
                        var products = await _context.Products
                            .Include(p => p.ProductImages)
                            .Include(p => p.Category)
                            .Include(p => p.Inventory)
                            .Where(p => itemIds.Contains(p.ProductCode ?? "") && p.IsActive == true)
                            .ToListAsync();

                        // Sắp xếp theo thứ tự recommendations
                        var productDict = products.ToDictionary(p => p.ProductCode ?? "");
                        var orderedProducts = apiResponse.Recommendations
                            .Where(r => productDict.ContainsKey(r.ItemId))
                            .Select(r => productDict[r.ItemId])
                            .Take(count)
                            .ToList();

                        return orderedProducts;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger?.LogError($"Local Recommendation API error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error calling Local Recommendation API: {ex.Message}");
            }

            return new List<Product>();
        }

        /// <summary>
        /// Lấy recommendations dựa trên danh sách sản phẩm đã mua (content-based)
        /// Sử dụng CBF model để tìm sản phẩm tương tự
        /// </summary>
        public async Task<List<Product>> GetContentBasedRecommendationsAsync(
            List<int> purchasedProductIds, 
            int count = 10)
        {
            try
            {
                if (!purchasedProductIds.Any())
                    return new List<Product>();

                // Kiểm tra API có sẵn sàng không
                if (!await IsApiAvailableAsync())
                {
                    _logger?.LogWarning("Local Recommendation API not available");
                    return new List<Product>();
                }

                // Lấy product code của sản phẩm đã mua
                var purchasedProducts = await _context.Products
                    .Where(p => purchasedProductIds.Contains(p.ProductID))
                    .Where(p => !string.IsNullOrEmpty(p.ProductCode))
                    .ToListAsync();

                if (!purchasedProducts.Any())
                    return new List<Product>();

                var baseUrl = GetApiBaseUrl();
                var allRecommendations = new List<Product>();
                var purchasedIdsSet = purchasedProductIds.ToHashSet();

                // Lấy recommendations cho từng sản phẩm đã mua (content-based)
                foreach (var product in purchasedProducts.Take(3)) // Giới hạn để tránh quá nhiều requests
                {
                    try
                    {
                        var requestBody = new
                        {
                            user_id = 0, // Không dùng user_id cho content-based
                            current_item_id = product.ProductCode,
                            k = count
                        };

                        var jsonContent = JsonSerializer.Serialize(requestBody);
                        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        var response = await _httpClient.PostAsync($"{baseUrl}/recommend", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var apiResponse = JsonSerializer.Deserialize<RecommendationApiResponse>(responseContent, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                            if (apiResponse?.Success == true && apiResponse.Recommendations != null)
                            {
                                var itemIds = apiResponse.Recommendations.Select(r => r.ItemId).ToList();
                                
                                var products = await _context.Products
                                    .Include(p => p.ProductImages)
                                    .Include(p => p.Category)
                                    .Include(p => p.Inventory)
                                    .Where(p => itemIds.Contains(p.ProductCode ?? "") && 
                                                p.IsActive == true &&
                                                !purchasedIdsSet.Contains(p.ProductID))
                                    .ToListAsync();

                                allRecommendations.AddRange(products);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning($"Error getting recommendations for product {product.ProductCode}: {ex.Message}");
                    }
                }

                // Remove duplicates và sắp xếp
                return allRecommendations
                    .GroupBy(p => p.ProductID)
                    .Select(g => g.First())
                    .Take(count)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error in GetContentBasedRecommendationsAsync: {ex.Message}");
                return new List<Product>();
            }
        }
    }

    // Models cho API response
    public class RecommendationApiResponse
    {
        public List<RecommendationItem>? Recommendations { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class RecommendationItem
    {
        public string ItemId { get; set; } = string.Empty;
        public double Score { get; set; }
    }
}

