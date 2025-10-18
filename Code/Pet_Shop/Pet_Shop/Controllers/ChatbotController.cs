using Microsoft.AspNetCore.Mvc;
using Pet_Shop.Services;
using Pet_Shop.Models.Entities;
using System.Text.Json;

namespace Pet_Shop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly ChatbotService _chatbotService;
        private readonly ILogger<ChatbotController> _logger;

        public ChatbotController(ChatbotService chatbotService, ILogger<ChatbotController> logger)
        {
            _chatbotService = chatbotService;
            _logger = logger;
        }

        /// <summary>
        /// API endpoint để gửi tin nhắn cho chatbot
        /// </summary>
        /// <param name="request">Request chứa tin nhắn và lịch sử chat</param>
        /// <returns>Phản hồi từ AI chatbot</returns>
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] ChatbotRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new { success = false, message = "Tin nhắn không được để trống" });
                }

                // Chuyển đổi lịch sử chat từ JSON nếu có
                List<ChatMessage>? conversationHistory = null;
                if (!string.IsNullOrEmpty(request.ConversationHistory))
                {
                    try
                    {
                        conversationHistory = JsonSerializer.Deserialize<List<ChatMessage>>(request.ConversationHistory);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning($"Failed to deserialize conversation history: {ex.Message}");
                    }
                }

                // Xử lý tin nhắn với AI
                var response = await _chatbotService.ProcessMessageAsync(request.Message, conversationHistory);

                return Ok(new
                {
                    success = response.IsSuccess,
                    message = response.Message,
                    suggestedProducts = response.SuggestedProducts.Select(p => new
                    {
                        productId = p.ProductID,
                        productName = p.ProductName,
                        price = p.Price,
                        salePrice = p.SalePrice,
                        shortDescription = p.ShortDescription,
                        categoryName = p.Category?.CategoryName,
                        imageUrl = p.ProductImages?.FirstOrDefault()?.ImageURL,
                        altText = p.ProductImages?.FirstOrDefault()?.AltText
                    }).ToList(),
                    error = response.Error
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing chatbot message: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi xử lý tin nhắn" });
            }
        }

        /// <summary>
        /// API endpoint để tìm kiếm sản phẩm thông minh
        /// </summary>
        /// <param name="query">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách sản phẩm phù hợp</returns>
        [HttpGet("search-products")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { success = false, message = "Từ khóa tìm kiếm không được để trống" });
                }

                var products = await _chatbotService.SearchProductsAsync(query);

                return Ok(new
                {
                    success = true,
                    products = products.Select(p => new
                    {
                        productId = p.ProductID,
                        productName = p.ProductName,
                        price = p.Price,
                        salePrice = p.SalePrice,
                        shortDescription = p.ShortDescription,
                        categoryName = p.Category?.CategoryName,
                        imageUrl = p.ProductImages?.FirstOrDefault()?.ImageURL,
                        altText = p.ProductImages?.FirstOrDefault()?.AltText
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching products: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi tìm kiếm sản phẩm" });
            }
        }

        /// <summary>
        /// API endpoint để lấy gợi ý sản phẩm phổ biến
        /// </summary>
        /// <returns>Danh sách sản phẩm phổ biến</returns>
        [HttpGet("popular-products")]
        public async Task<IActionResult> GetPopularProducts()
        {
            try
            {
                var products = await _chatbotService.SearchProductsAsync("thức ăn chó mèo phổ biến");
                
                return Ok(new
                {
                    success = true,
                    products = products.Take(6).Select(p => new
                    {
                        productId = p.ProductID,
                        productName = p.ProductName,
                        price = p.Price,
                        salePrice = p.SalePrice,
                        shortDescription = p.ShortDescription,
                        categoryName = p.Category?.CategoryName,
                        imageUrl = p.ProductImages?.FirstOrDefault()?.ImageURL,
                        altText = p.ProductImages?.FirstOrDefault()?.AltText
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting popular products: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi lấy sản phẩm phổ biến" });
            }
        }

        /// <summary>
        /// API endpoint để lấy thông tin chatbot
        /// </summary>
        /// <returns>Thông tin về chatbot</returns>
        [HttpGet("info")]
        public IActionResult GetChatbotInfo()
        {
            return Ok(new
            {
                success = true,
                name = "HyHy Pet Shop Assistant",
                description = "Trợ lý AI thông minh giúp bạn tìm sản phẩm phù hợp cho thú cưng",
                capabilities = new[]
                {
                    "Tư vấn sản phẩm cho chó và mèo",
                    "Gợi ý thức ăn phù hợp",
                    "Tìm phụ kiện thú cưng",
                    "Tư vấn dinh dưỡng",
                    "Hỗ trợ tìm kiếm theo giá cả"
                },
                version = "1.0.0"
            });
        }
    }

    /// <summary>
    /// Model cho request gửi tin nhắn chatbot
    /// </summary>
    public class ChatbotRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? ConversationHistory { get; set; }
    }
}
