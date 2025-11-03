using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;
using System.Text.Json;
using System.Text;
using System.Linq;

namespace Pet_Shop.Services
{
    public class ChatbotService
    {
        private readonly PetShopDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public ChatbotService(PetShopDbContext context, IConfiguration configuration, ProductService productService, CategoryService categoryService, HttpClient httpClient, IMemoryCache memoryCache)
        {
            _context = context;
            _configuration = configuration;
            _productService = productService;
            _categoryService = categoryService;
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Xử lý tin nhắn từ người dùng và trả về phản hồi AI
        /// </summary>
        /// <param name="userMessage">Tin nhắn từ người dùng</param>
        /// <param name="conversationHistory">Lịch sử cuộc trò chuyện</param>
        /// <returns>Phản hồi từ AI</returns>
        public async Task<ChatbotResponse> ProcessMessageAsync(string userMessage, List<ChatMessage>? conversationHistory = null)
        {
            try
            {
                // Lấy thông tin database để cung cấp context cho AI
                var databaseContext = await GetDatabaseContextAsync();
                
                // Tạo system prompt với thông tin về shop và database
                var systemPrompt = CreateSystemPrompt(databaseContext);
                
                // Tạo request cho OpenAI API
                var requestBody = new
                {
                    model = _configuration["OpenAISettings:Model"] ?? "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userMessage }
                    },
                    max_tokens = int.Parse(_configuration["OpenAISettings:MaxTokens"] ?? "1000"),
                    temperature = float.Parse(_configuration["OpenAISettings:Temperature"] ?? "0.7")
                };

                var apiKey = _configuration["OpenAISettings:ApiKey"];
                if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_OPENAI_API_KEY_HERE")
                {
                    // Fallback: Sử dụng OpenAI với database context
                    return await ProcessMessageWithOpenAIAndDatabaseAsync(userMessage, databaseContext, conversationHistory);
                }

                // Gọi OpenAI API
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var openAIResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);
                    
                    var aiResponse = openAIResponse?.choices?.FirstOrDefault()?.message?.content ?? "Xin lỗi, tôi không thể xử lý yêu cầu của bạn.";
                    
                    // Phân tích phản hồi để tìm sản phẩm được đề xuất
                    var suggestedProducts = await ExtractSuggestedProductsAsync(aiResponse, userMessage);

                    return new ChatbotResponse
                    {
                        Message = aiResponse,
                        SuggestedProducts = suggestedProducts,
                        IsSuccess = true
                    };
                }
                else
                {
                    // Fallback nếu API không hoạt động
                    return await ProcessMessageWithFallbackAsync(userMessage, databaseContext);
                }
            }
            catch (Exception ex)
            {
                // Fallback nếu có lỗi
                try
                {
                    var databaseContext = await GetDatabaseContextAsync();
                    return await ProcessMessageWithFallbackAsync(userMessage, databaseContext);
                }
                catch
                {
                    return new ChatbotResponse
                    {
                        Message = "Xin lỗi, có lỗi xảy ra khi xử lý yêu cầu của bạn. Vui lòng thử lại sau.",
                        SuggestedProducts = new List<Product>(),
                        IsSuccess = false,
                        Error = ex.Message
                    };
                }
            }
        }

        /// <summary>
        /// Xử lý tin nhắn với OpenAI và database context (fallback khi không có API key)
        /// </summary>
        private async Task<ChatbotResponse> ProcessMessageWithOpenAIAndDatabaseAsync(string userMessage, DatabaseContext context, List<ChatMessage>? conversationHistory = null)
        {
            try
            {
                // Tạo system prompt với thông tin database chi tiết
                var systemPrompt = CreateDetailedSystemPrompt(context);
                
                // Tạo request cho OpenAI API với database context
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userMessage }
                    },
                    max_tokens = 1000,
                    temperature = 0.7
                };

                // Gọi OpenAI API
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["OpenAISettings:ApiKey"]}");
                
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var openAIResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);
                    
                    var aiResponse = openAIResponse?.choices?.FirstOrDefault()?.message?.content ?? "Xin lỗi, tôi không thể xử lý yêu cầu của bạn.";
                    
                    // Phân tích phản hồi AI để tìm sản phẩm được đề xuất
                    var suggestedProducts = await ExtractSuggestedProductsFromAIResponseAsync(aiResponse, userMessage, context);

                    return new ChatbotResponse
                    {
                        Message = aiResponse,
                        SuggestedProducts = suggestedProducts,
                        IsSuccess = true
                    };
                }
                else
                {
                    // Fallback nếu API không hoạt động
                    return await ProcessMessageWithFallbackAsync(userMessage, context);
                }
            }
            catch (Exception)
            {
                // Fallback nếu có lỗi
                return await ProcessMessageWithFallbackAsync(userMessage, context);
            }
        }

        /// <summary>
        /// Xử lý tin nhắn với logic fallback (không cần OpenAI)
        /// </summary>
        private async Task<ChatbotResponse> ProcessMessageWithFallbackAsync(string userMessage, DatabaseContext context)
        {
            var message = userMessage.ToLower();
            var suggestedProducts = new List<Product>();
            var response = "";

            // Kiểm tra các câu chào hỏi và câu hỏi chung
            if (IsGreetingMessage(message))
            {
                response = "Xin chào! Tôi là trợ lý AI của HyHy Pet Shop. Tôi có thể giúp bạn tìm sản phẩm phù hợp cho thú cưng. Bạn cần tìm gì hôm nay?";
                return new ChatbotResponse
                {
                    Message = response,
                    SuggestedProducts = suggestedProducts,
                    IsSuccess = true
                };
            }

            if (IsGeneralQuestion(message))
            {
                response = GetGeneralResponse(message);
                return new ChatbotResponse
                {
                    Message = response,
                    SuggestedProducts = suggestedProducts,
                    IsSuccess = true
                };
            }

            // Chỉ gợi ý sản phẩm khi thực sự cần thiết
            if (IsProductRelatedQuery(message))
            {
                // Phân tích yêu cầu chi tiết
                var searchCriteria = AnalyzeSearchCriteria(message);
                
                // Tìm kiếm sản phẩm dựa trên tiêu chí
                suggestedProducts = await SearchProductsWithCriteriaAsync(searchCriteria);
                
                // Tạo phản hồi phù hợp
                response = GenerateResponseForSearchCriteria(searchCriteria, suggestedProducts);
            }
            else
            {
                response = "Tôi hiểu bạn đang hỏi về điều gì đó. Tôi có thể giúp bạn tìm sản phẩm cho thú cưng. Bạn có thể hỏi về thức ăn, phụ kiện, hoặc sản phẩm cho chó/mèo.";
            }

            return new ChatbotResponse
            {
                Message = response,
                SuggestedProducts = suggestedProducts,
                IsSuccess = true
            };
        }

        /// <summary>
        /// Kiểm tra xem có phải câu chào hỏi không
        /// </summary>
        private bool IsGreetingMessage(string message)
        {
            var greetings = new[] { "xin chào", "chào", "hello", "hi", "hey", "chào bạn", "chào anh", "chào chị", "good morning", "good afternoon", "good evening" };
            return greetings.Any(greeting => message.Contains(greeting));
        }

        /// <summary>
        /// Kiểm tra xem có phải câu hỏi chung không
        /// </summary>
        private bool IsGeneralQuestion(string message)
        {
            var generalQuestions = new[] { "bạn là ai", "bạn có thể làm gì", "giúp tôi", "hướng dẫn", "cách sử dụng", "tính năng", "chức năng" };
            return generalQuestions.Any(question => message.Contains(question));
        }

        /// <summary>
        /// Kiểm tra xem có phải câu hỏi về sản phẩm không
        /// </summary>
        private bool IsProductRelatedQuery(string message)
        {
            var productKeywords = new[] { 
                "sản phẩm", "mua", "bán", "giá", "thức ăn", "phụ kiện", "chó", "mèo", "dog", "cat", 
                "thú cưng", "pet", "đồ chơi", "toy", "chuồng", "cage", "dây dắt", "leash",
                "khuyến mãi", "giảm giá", "sale", "discount", "tìm kiếm", "search", "gợi ý", "suggest",
                "kg", "kilogram", "gram", "trọng lượng", "kích thước", "size", "dưới", "trên", "khoảng"
            };
            return productKeywords.Any(keyword => message.Contains(keyword));
        }

        /// <summary>
        /// Trả lời câu hỏi chung
        /// </summary>
        private string GetGeneralResponse(string message)
        {
            if (message.Contains("bạn là ai") || message.Contains("bạn có thể làm gì"))
            {
                return "Tôi là trợ lý AI của HyHy Pet Shop. Tôi có thể giúp bạn:\n" +
                       "🐕 Tìm sản phẩm cho chó\n" +
                       "🐱 Tìm sản phẩm cho mèo\n" +
                       "🍖 Tư vấn thức ăn phù hợp\n" +
                       "🎾 Gợi ý phụ kiện thú cưng\n" +
                       "💰 Tìm sản phẩm khuyến mãi\n\n" +
                       "Bạn cần tìm gì hôm nay?";
            }
            
            if (message.Contains("giúp tôi") || message.Contains("hướng dẫn"))
            {
                return "Tôi có thể giúp bạn tìm sản phẩm phù hợp cho thú cưng. Hãy cho tôi biết:\n" +
                       "• Bạn có chó hay mèo?\n" +
                       "• Bạn cần thức ăn hay phụ kiện?\n" +
                       "• Ngân sách của bạn là bao nhiêu?\n\n" +
                       "Tôi sẽ gợi ý sản phẩm phù hợp nhất!";
            }

            return "Tôi có thể giúp bạn tìm sản phẩm cho thú cưng. Bạn có thể hỏi về thức ăn, phụ kiện, hoặc sản phẩm cho chó/mèo.";
        }

        /// <summary>
        /// Phân tích tiêu chí tìm kiếm từ tin nhắn
        /// </summary>
        private SearchCriteria AnalyzeSearchCriteria(string message)
        {
            var criteria = new SearchCriteria();
            var lowerMessage = message.ToLower();

            // Phân tích loại thú cưng
            if (lowerMessage.Contains("chó") || lowerMessage.Contains("dog"))
            {
                criteria.PetType = "chó";
            }
            else if (lowerMessage.Contains("mèo") || lowerMessage.Contains("cat"))
            {
                criteria.PetType = "mèo";
            }

            // Phân tích loại sản phẩm
            if (lowerMessage.Contains("thức ăn") || lowerMessage.Contains("food"))
            {
                criteria.ProductType = "thức ăn";
            }
            else if (lowerMessage.Contains("phụ kiện") || lowerMessage.Contains("accessory"))
            {
                criteria.ProductType = "phụ kiện";
            }
            else if (lowerMessage.Contains("đồ chơi") || lowerMessage.Contains("toy"))
            {
                criteria.ProductType = "đồ chơi";
            }

            // Phân tích giá cả
            var pricePatterns = new[]
            {
                @"dưới\s+(\d+)",
                @"dưới\s+(\d+)\s*k",
                @"dưới\s+(\d+)\s*000",
                @"dưới\s+(\d+)\s*000\s*000",
                @"dưới\s+(\d+)\s*tr",
                @"dưới\s+(\d+)\s*triệu"
            };

            foreach (var pattern in pricePatterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(lowerMessage, pattern);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int price))
                {
                    criteria.MaxPrice = price switch
                    {
                        var p when p < 100 => p * 1000, // Nếu < 100 thì nhân 1000 (VD: 200 -> 200k)
                        var p when p < 1000 => p * 1000, // Nếu < 1000 thì nhân 1000 (VD: 500 -> 500k)
                        _ => price // Nếu >= 1000 thì giữ nguyên (VD: 200000)
                    };
                    break;
                }
            }

            // Phân tích trọng lượng/kích thước
            var weightPatterns = new[]
            {
                @"(\d+)\s*kg",
                @"(\d+)\s*kilogram",
                @"(\d+)\s*gram",
                @"trọng\s*lượng\s*(\d+)"
            };

            foreach (var pattern in weightPatterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(lowerMessage, pattern);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int weight))
                {
                    criteria.Weight = weight;
                    break;
                }
            }

            // Phân tích khuyến mãi
            if (lowerMessage.Contains("giảm giá") || lowerMessage.Contains("khuyến mãi") || lowerMessage.Contains("sale"))
            {
                criteria.OnSale = true;
            }

            return criteria;
        }

        /// <summary>
        /// Tìm kiếm sản phẩm dựa trên tiêu chí
        /// </summary>
        private async Task<List<Product>> SearchProductsWithCriteriaAsync(SearchCriteria criteria)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.IsActive);

            // Lọc theo loại thú cưng
            if (!string.IsNullOrEmpty(criteria.PetType))
            {
                query = query.Where(p => 
                    p.ProductName.Contains(criteria.PetType) || 
                    p.Category.CategoryName.Contains(criteria.PetType));
            }

            // Lọc theo loại sản phẩm
            if (!string.IsNullOrEmpty(criteria.ProductType))
            {
                query = query.Where(p => p.ProductName.Contains(criteria.ProductType));
            }

            // Lọc theo giá
            if (criteria.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= criteria.MaxPrice.Value);
            }

            // Lọc theo khuyến mãi
            if (criteria.OnSale)
            {
                query = query.Where(p => p.SalePrice.HasValue && p.SalePrice < p.Price);
            }

            // Sắp xếp theo giá tăng dần nếu có tiêu chí giá
            if (criteria.MaxPrice.HasValue)
            {
                query = query.OrderBy(p => p.Price);
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedDate);
            }

            return await query.Take(5).ToListAsync();
        }

        /// <summary>
        /// Tạo phản hồi dựa trên tiêu chí tìm kiếm
        /// </summary>
        private string GenerateResponseForSearchCriteria(SearchCriteria criteria, List<Product> products)
        {
            if (!products.Any())
            {
                return "Xin lỗi, tôi không tìm thấy sản phẩm nào phù hợp với yêu cầu của bạn. Bạn có thể thử tìm kiếm với tiêu chí khác không?";
            }

            var response = "Tôi tìm thấy một số sản phẩm phù hợp với yêu cầu của bạn:\n\n";

            // Thêm thông tin về tiêu chí tìm kiếm
            if (!string.IsNullOrEmpty(criteria.PetType))
            {
                response += $"🐕 Dành cho: {criteria.PetType}\n";
            }

            if (!string.IsNullOrEmpty(criteria.ProductType))
            {
                response += $"📦 Loại: {criteria.ProductType}\n";
            }

            if (criteria.MaxPrice.HasValue)
            {
                response += $"💰 Giá dưới: {criteria.MaxPrice.Value:N0} ₫\n";
            }

            if (criteria.Weight.HasValue)
            {
                response += $"⚖️ Trọng lượng: {criteria.Weight} kg\n";
            }

            if (criteria.OnSale)
            {
                response += $"🎉 Đang khuyến mãi\n";
            }

            response += "\nDưới đây là những gợi ý tốt nhất:";

            return response;
        }

        /// <summary>
        /// Lấy thông tin database để cung cấp context cho AI
        /// </summary>
        private async Task<DatabaseContext> GetDatabaseContextAsync()
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Take(50) // Giới hạn để tránh quá tải
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();

            var brands = await _context.Brands
                .Where(b => b.IsActive)
                .ToListAsync();

            return new DatabaseContext
            {
                Products = products,
                Categories = categories,
                Brands = brands
            };
        }

        /// <summary>
        /// Tạo system prompt chi tiết với thông tin database
        /// </summary>
        private string CreateDetailedSystemPrompt(DatabaseContext context)
        {
            var productList = string.Join("\n", context.Products.Select(p => 
                $"- {p.ProductName} (ID: {p.ProductID}, Giá: {p.Price:N0}₫, Giá sale: {(p.SalePrice?.ToString("N0") ?? "N/A")}₫, Danh mục: {p.Category?.CategoryName}, Mô tả: {p.ShortDescription}, Trọng lượng: {(p.Weight?.ToString() ?? "N/A")}kg)"));

            var categoryList = string.Join(", ", context.Categories.Select(c => c.CategoryName));
            var brandList = string.Join(", ", context.Brands.Select(b => b.BrandName));

            return $@"Bạn là HyHy - trợ lý AI thông minh và thân thiện của HyHy Pet Shop, một cửa hàng chuyên nghiệp về thức ăn và phụ kiện cho thú cưng.

═══════════════════════════════════════════════════════════════
🛍️ VỀ CỬA HÀNG
═══════════════════════════════════════════════════════════════
- Tên: HyHy Pet Shop
- Chuyên: Thức ăn và phụ kiện chất lượng cao cho chó, mèo
- Sứ mệnh: Mang lại sản phẩm tốt nhất cho thú cưng của bạn
- Đặc điểm: Sản phẩm chính hãng, giá cả phải chăng, dịch vụ tận tâm

═══════════════════════════════════════════════════════════════
📦 KHO HÀNG HIỆN TẠI (Sử dụng thông tin này để tư vấn chính xác)
═══════════════════════════════════════════════════════════════

DANH MỤC SẢN PHẨM: {categoryList}

THƯƠNG HIỆU: {brandList}

SẢN PHẨM CÓ SẴN:
{productList}

═══════════════════════════════════════════════════════════════
🎯 NHIỆM VỤ CỦA BẠN
═══════════════════════════════════════════════════════════════

1. TƯ VẤN SẢN PHẨM THÔNG MINH
   ✓ Phân tích nhu cầu khách hàng một cách chi tiết
   ✓ Gợi ý sản phẩm phù hợp dựa trên: loại thú cưng, độ tuổi, trọng lượng, ngân sách
   ✓ So sánh sản phẩm và giải thích lợi ích
   ✓ Đề xuất sản phẩm thay thế nếu cần

2. HỖ TRỢ TÌM KIẾM
   ✓ Tìm theo giá cả, thương hiệu, danh mục
   ✓ Tìm sản phẩm khuyến mãi, giảm giá
   ✓ Tìm sản phẩm theo trọng lượng/kích thước

3. TƯ VẤN DINH DƯỠNG & CHĂM SÓC
   ✓ Tư vấn thức ăn phù hợp cho từng độ tuổi
   ✓ Gợi ý khẩu phần ăn
   ✓ Tư vấn về dinh dưỡng đặc biệt (weight management, sensitive stomach, etc.)
   ✓ Gợi ý phụ kiện cần thiết

4. TRẢI NGHIỆM KHÁCH HÀNG
   ✓ Luôn thân thiện, nhiệt tình, chuyên nghiệp
   ✓ Trả lời rõ ràng, dễ hiểu
   ✓ Hỏi thêm thông tin khi cần thiết
   ✓ Đề xuất giải pháp thay thế

═══════════════════════════════════════════════════════════════
⚙️ QUY TẮC GIAO TIẾP QUAN TRỌNG
═══════════════════════════════════════════════════════════════

✅ NÊN LÀM:
- Trả lời bằng tiếng Việt tự nhiên, thân thiện
- Sử dụng emoji phù hợp để tạo cảm giác thân thiện (nhưng không lạm dụng)
- CHỈ gợi ý sản phẩm khi khách hàng thực sự hỏi về sản phẩm hoặc tìm kiếm
- Khi gợi ý sản phẩm, LUÔN đề cập ID sản phẩm (ví dụ: ""Sản phẩm ID: 1 - Royal Canin Adult"")
- Giải thích lý do đề xuất dựa trên thông tin thực tế
- Đưa ra 2-3 lựa chọn để khách hàng tham khảo
- Hỏi thêm thông tin nếu cần thiết (độ tuổi, trọng lượng, ngân sách)
- Ưu tiên sản phẩm có khuyến mãi nếu phù hợp

❌ KHÔNG NÊN:
- KHÔNG gợi ý sản phẩm khi khách hàng chỉ chào hỏi hoặc hỏi thông tin chung
- KHÔNG gợi ý sản phẩm không có trong danh sách
- KHÔNG đưa ra thông tin sai về giá cả, thông số kỹ thuật
- KHÔNG tạo áp lực bán hàng hoặc spam gợi ý
- KHÔNG sử dụng ngôn ngữ quá trang trọng hay quá informal

═══════════════════════════════════════════════════════════════
📋 CÁC TRƯỜNG HỢP XỬ LÝ CỤ THỂ
═══════════════════════════════════════════════════════════════

1️⃣ CHÀO HỎI (KHÔNG gợi ý sản phẩm)
   Người dùng: ""Xin chào"", ""Hello"", ""Hi""
   Phản hồi: Chào hỏi thân thiện, giới thiệu về bản thân và khả năng, HỎI khách hàng cần gì
   Ví dụ: ""Xin chào! Tôi là HyHy, trợ lý AI của HyHy Pet Shop. Tôi có thể giúp bạn tìm sản phẩm phù hợp cho thú cưng. Bạn có chó hay mèo?""

2️⃣ HỎI THÔNG TIN (KHÔNG gợi ý sản phẩm)
   Người dùng: ""Bạn là ai?"", ""Bạn có thể làm gì?"", ""Giờ mở cửa?""
   Phản hồi: Trả lời thông tin, KHÔNG ép sản phẩm
   Ví dụ: ""Tôi là trợ lý AI của HyHy Pet Shop. Tôi có thể giúp bạn tìm thức ăn, phụ kiện cho thú cưng. Bạn cần tìm gì hôm nay?""

3️⃣ TÌM KIẾM SẢN PHẨM (CÓ gợi ý sản phẩm)
   Người dùng: ""Tôi cần thức ăn cho chó"", ""Sản phẩm nào cho mèo?"", ""Tìm phụ kiện""
   Phản hồi: Phân tích nhu cầu, hỏi thêm thông tin nếu cần, đưa ra 2-3 gợi ý với ID
   Ví dụ: ""Để tôi tìm sản phẩm phù hợp cho bạn. Bạn có chó con hay chó trưởng thành? Trọng lượng bao nhiêu kg? Ngân sách của bạn là bao nhiêu?""

4️⃣ TÌM KIẾM THEO GIÁ (CÓ gợi ý sản phẩm)
   Người dùng: ""Sản phẩm nào dưới 200k?"", ""Khuyến mãi gì?""
   Phản hồi: Tìm sản phẩm phù hợp với ngân sách, ƯU TIÊN sản phẩm khuyến mãi
   Ví dụ: ""Tôi tìm thấy một số sản phẩm phù hợp với ngân sách của bạn: [Danh sách sản phẩm với ID]""

5️⃣ YÊU CẦU CỤ THỂ (CÓ gợi ý sản phẩm)
   Người dùng: ""Thức ăn cho chó 5kg"", ""Phụ kiện cho mèo con""
   Phản hồi: Gợi ý sản phẩm phù hợp ngay lập tức
   Ví dụ: ""Tôi tìm thấy sản phẩm phù hợp: Sản phẩm ID: 1 - Royal Canin Adult 5kg - Giá: 400,000₫ (Giá sale: 350,000₫)...""

═══════════════════════════════════════════════════════════════
💡 HƯỚNG DẪN ĐỀ XUẤT SẢN PHẨM
═══════════════════════════════════════════════════════════════

Khi đề xuất sản phẩm, hãy tuân theo format sau:

```
Dựa trên nhu cầu của bạn, tôi đề xuất:

🏷️ Sản phẩm ID: [ID] - [Tên sản phẩm]
💰 Giá: [Giá]₫ [Giá sale nếu có]
📦 Danh mục: [Danh mục]
⚖️ Trọng lượng: [Trọng lượng]kg
📝 Mô tả: [Mô tả ngắn gọn]
💡 Lý do đề xuất: [Giải thích vì sao phù hợp]

[Lặp lại cho 2-3 sản phẩm]
```

═══════════════════════════════════════════════════════════════
🎨 PHONG CÁCH GIAO TIẾP
═══════════════════════════════════════════════════════════════

- Thân thiện như một người bạn
- Chuyên nghiệp như một nhân viên tư vấn
- Nhiệt tình nhưng không ép buộc
- Rõ ràng, dễ hiểu
- Sử dụng emoji vừa phải để tạo điểm nhấn

═══════════════════════════════════════════════════════════════

Hãy nhớ: Mục tiêu của bạn là giúp khách hàng TÌM ĐÚNG sản phẩm họ cần, không phải bán càng nhiều càng tốt. Trải nghiệm khách hàng là ưu tiên hàng đầu! 🎯";
        }

        /// <summary>
        /// Tạo system prompt với thông tin về shop
        /// </summary>
        private string CreateSystemPrompt(DatabaseContext context)
        {
            var productList = string.Join("\n", context.Products.Select(p => 
                $"- {p.ProductName} (ID: {p.ProductID}, Giá: {p.Price:N0}₫, Danh mục: {p.Category?.CategoryName}, Mô tả: {p.ShortDescription})"));

            var categoryList = string.Join(", ", context.Categories.Select(c => c.CategoryName));

            return $@"Bạn là một trợ lý AI thông minh của HyHy Pet Shop - cửa hàng chuyên cung cấp thức ăn và phụ kiện cho thú cưng.

THÔNG TIN CỬA HÀNG:
- Tên: HyHy Pet Shop
- Chuyên: Thức ăn và phụ kiện cho chó, mèo
- Sản phẩm chất lượng cao từ các thương hiệu uy tín

DANH SÁCH SẢN PHẨM HIỆN CÓ:
{productList}

DANH MỤC SẢN PHẨM:
{categoryList}

NHIỆM VỤ CỦA BẠN:
1. Tư vấn sản phẩm phù hợp dựa trên nhu cầu của khách hàng
2. Gợi ý sản phẩm theo loại thú cưng (chó, mèo)
3. Tư vấn về dinh dưỡng và chăm sóc thú cưng
4. Hỗ trợ tìm kiếm sản phẩm theo giá cả, thương hiệu
5. Luôn thân thiện, chuyên nghiệp và hữu ích

QUY TẮC TRẢ LỜI QUAN TRỌNG:
- Trả lời bằng tiếng Việt
- Thân thiện và chuyên nghiệp
- CHỈ gợi ý sản phẩm khi khách hàng thực sự hỏi về sản phẩm
- KHÔNG gợi ý sản phẩm khi khách hàng chỉ chào hỏi hoặc hỏi chung
- Khi gợi ý sản phẩm, đề xuất sản phẩm cụ thể với ID và tên
- Giải thích lý do đề xuất
- Hỏi thêm thông tin nếu cần thiết

CÁC TRƯỜNG HỢP KHÔNG GỢI Ý SẢN PHẨM:
- Chào hỏi: xin chào, hello, hi
- Câu hỏi chung: bạn là ai, bạn có thể làm gì
- Hỏi thông tin: giờ mở cửa, địa chỉ, liên hệ

CÁC TRƯỜNG HỢP NÊN GỢI Ý SẢN PHẨM:
- Hỏi về sản phẩm: tôi cần thức ăn cho chó, sản phẩm nào cho mèo
- Tìm kiếm: tìm phụ kiện, đồ chơi cho chó
- Giá cả: sản phẩm nào rẻ, khuyến mãi gì

Khi đề xuất sản phẩm, hãy đề cập đến ID sản phẩm để hệ thống có thể hiển thị chi tiết.";
        }

        /// <summary>
        /// Phân tích phản hồi AI để tìm sản phẩm được đề xuất (từ AI response)
        /// </summary>
        private async Task<List<Product>> ExtractSuggestedProductsFromAIResponseAsync(string aiResponse, string userMessage, DatabaseContext context)
        {
            var suggestedProducts = new List<Product>();

            try
            {
                // Tìm ID sản phẩm trong phản hồi AI (ví dụ: "ID: 1", "sản phẩm 2", "product 3")
                var productIdPattern = @"(?:ID:\s*|sản phẩm\s+|product\s+)(\d+)";
                var matches = System.Text.RegularExpressions.Regex.Matches(aiResponse, productIdPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    if (int.TryParse(match.Groups[1].Value, out int productId))
                    {
                        var product = await _context.Products
                            .Include(p => p.Category)
                            .Include(p => p.Brand)
                            .FirstOrDefaultAsync(p => p.ProductID == productId);

                        if (product != null && !suggestedProducts.Any(p => p.ProductID == productId))
                        {
                            suggestedProducts.Add(product);
                        }
                    }
                }

                // Nếu không tìm thấy ID, tìm kiếm theo tên sản phẩm
                if (suggestedProducts.Count == 0)
                {
                    var productNames = context.Products.Select(p => p.ProductName).ToList();
                    foreach (var productName in productNames)
                    {
                        if (aiResponse.Contains(productName, StringComparison.OrdinalIgnoreCase))
                        {
                            var product = await _context.Products
                                .Include(p => p.Category)
                                .Include(p => p.Brand)
                                .FirstOrDefaultAsync(p => p.ProductName == productName);

                            if (product != null && !suggestedProducts.Any(p => p.ProductID == product.ProductID))
                            {
                                suggestedProducts.Add(product);
                            }
                        }
                    }
                }

                // Nếu vẫn không tìm thấy, tìm kiếm theo từ khóa
                if (suggestedProducts.Count == 0)
                {
                    suggestedProducts = await SearchProductsByKeywordsAsync(userMessage, context);
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không throw
                Console.WriteLine($"Error extracting suggested products from AI response: {ex.Message}");
            }

            return suggestedProducts.Take(3).ToList();
        }

        /// <summary>
        /// Phân tích phản hồi AI để tìm sản phẩm được đề xuất
        /// </summary>
        private async Task<List<Product>> ExtractSuggestedProductsAsync(string aiResponse, string userMessage)
        {
            var suggestedProducts = new List<Product>();
            
            try
            {
                // Tìm ID sản phẩm trong phản hồi AI
                var productIdMatches = System.Text.RegularExpressions.Regex.Matches(aiResponse, @"ID:\s*(\d+)");
                
                foreach (System.Text.RegularExpressions.Match match in productIdMatches)
                {
                    if (int.TryParse(match.Groups[1].Value, out int productId))
                    {
                        var product = await _context.Products
                            .Include(p => p.Category)
                            .Include(p => p.ProductImages)
                            .FirstOrDefaultAsync(p => p.ProductID == productId && p.IsActive);
                        
                        if (product != null && !suggestedProducts.Any(p => p.ProductID == productId))
                        {
                            suggestedProducts.Add(product);
                        }
                    }
                }

                // Nếu không tìm thấy ID cụ thể, tìm kiếm theo từ khóa
                if (!suggestedProducts.Any())
                {
                    var keywords = ExtractKeywords(userMessage);
                    if (keywords.Any())
                    {
                        var products = await _context.Products
                            .Include(p => p.Category)
                            .Include(p => p.ProductImages)
                            .Where(p => p.IsActive && 
                                (keywords.Any(k => p.ProductName.Contains(k)) ||
                                 keywords.Any(k => p.ShortDescription != null && p.ShortDescription.Contains(k)) ||
                                 keywords.Any(k => p.Category != null && p.Category.CategoryName.Contains(k))))
                            .Take(3)
                            .ToListAsync();
                        
                        suggestedProducts.AddRange(products);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the entire response
                Console.WriteLine($"Error extracting suggested products: {ex.Message}");
            }

            return suggestedProducts;
        }

        /// <summary>
        /// Trích xuất từ khóa từ tin nhắn người dùng
        /// </summary>
        private List<string> ExtractKeywords(string message)
        {
            var keywords = new List<string>();
            var words = message.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            // Từ khóa liên quan đến thú cưng
            var petKeywords = new[] { "chó", "mèo", "dog", "cat", "puppy", "kitten", "adult", "trưởng thành", "con" };
            var foodKeywords = new[] { "thức ăn", "food", "ăn", "dinh dưỡng", "nutrition" };
            var accessoryKeywords = new[] { "phụ kiện", "accessory", "đồ chơi", "toy", "chuồng", "cage", "dây dắt", "leash" };
            
            keywords.AddRange(words.Where(w => petKeywords.Any(pk => w.Contains(pk))));
            keywords.AddRange(words.Where(w => foodKeywords.Any(fk => w.Contains(fk))));
            keywords.AddRange(words.Where(w => accessoryKeywords.Any(ak => w.Contains(ak))));
            
            return keywords.Distinct().ToList();
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo từ khóa với database context
        /// </summary>
        private async Task<List<Product>> SearchProductsByKeywordsAsync(string userMessage, DatabaseContext context)
        {
            try
            {
                var keywords = ExtractKeywords(userMessage);
                if (!keywords.Any())
                {
                    return new List<Product>();
                }

                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Where(p => p.IsActive && 
                        (keywords.Any(k => p.ProductName.Contains(k)) ||
                         keywords.Any(k => p.ShortDescription != null && p.ShortDescription.Contains(k)) ||
                         keywords.Any(k => p.Category != null && p.Category.CategoryName.Contains(k))))
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(3)
                    .ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching products by keywords: {ex.Message}");
                return new List<Product>();
            }
        }

        /// <summary>
        /// Tìm kiếm sản phẩm thông minh dựa trên mô tả
        /// </summary>
        public async Task<List<Product>> SearchProductsAsync(string description)
        {
            try
            {
                var keywords = ExtractKeywords(description);
                if (!keywords.Any())
                {
                    return new List<Product>();
                }

                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Where(p => p.IsActive && 
                        (keywords.Any(k => p.ProductName.Contains(k)) ||
                         keywords.Any(k => p.ShortDescription != null && p.ShortDescription.Contains(k)) ||
                         keywords.Any(k => p.Category != null && p.Category.CategoryName.Contains(k))))
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(10)
                    .ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching products: {ex.Message}");
                return new List<Product>();
            }
        }

        /// <summary>
        /// Tạo embeddings cho sản phẩm sử dụng OpenAI API
        /// </summary>
        private async Task<float[]?> GetProductEmbeddingAsync(Product product)
        {
            try
            {
                // Kiểm tra cache trước
                var cacheKey = $"product_embedding_{product.ProductID}";
                if (_memoryCache.TryGetValue(cacheKey, out float[]? cachedEmbedding) && cachedEmbedding != null)
                {
                    return cachedEmbedding;
                }

                var apiKey = _configuration["OpenAISettings:ApiKey"];
                var useEmbeddings = _configuration.GetValue<bool>("OpenAISettings:UseEmbeddings", false);
                
                // Nếu không có API key hoặc embeddings bị tắt, return null
                if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_OPENAI_API_KEY_HERE" || !useEmbeddings)
                {
                    return null;
                }

                // Tạo text để embedding: kết hợp tên, mô tả, category, brand, pet type
                var embeddingText = BuildProductEmbeddingText(product);

                // Gọi OpenAI Embeddings API
                var embeddingModel = _configuration["OpenAISettings:EmbeddingModel"] ?? "text-embedding-3-small";
                
                var requestBody = new
                {
                    input = embeddingText,
                    model = embeddingModel
                };

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/embeddings", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    // Deserialize JSON với JsonDocument để xử lý embedding array
                    using (var doc = JsonDocument.Parse(responseContent))
                    {
                        var root = doc.RootElement;
                        if (root.TryGetProperty("data", out var dataArray) && dataArray.GetArrayLength() > 0)
                        {
                            var firstItem = dataArray[0];
                            if (firstItem.TryGetProperty("embedding", out var embeddingArray))
                            {
                                var embedding = embeddingArray.EnumerateArray()
                                    .Select(x => (float)x.GetDouble())
                                    .ToArray();
                                
                                // Cache embedding trong 24 giờ
                                var cacheOptions = new MemoryCacheEntryOptions
                                {
                                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                                };
                                _memoryCache.Set(cacheKey, embedding, cacheOptions);
                                
                                return embedding;
                            }
                        }
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting embedding for product {product.ProductID}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Xây dựng text để tạo embedding cho sản phẩm
        /// </summary>
        private string BuildProductEmbeddingText(Product product)
        {
            var parts = new List<string>();
            
            parts.Add(product.ProductName);
            
            if (!string.IsNullOrEmpty(product.ShortDescription))
            {
                parts.Add(product.ShortDescription);
            }
            
            if (!string.IsNullOrEmpty(product.Description))
            {
                // Chỉ lấy 500 ký tự đầu của description để tránh quá dài
                var description = product.Description.Length > 500 
                    ? product.Description.Substring(0, 500) 
                    : product.Description;
                parts.Add(description);
            }
            
            if (product.Category != null)
            {
                parts.Add($"Danh mục: {product.Category.CategoryName}");
            }
            
            if (product.Brand != null)
            {
                parts.Add($"Thương hiệu: {product.Brand.BrandName}");
            }
            
            if (!string.IsNullOrEmpty(product.PetType))
            {
                parts.Add($"Cho thú cưng: {product.PetType}");
            }
            
            if (!string.IsNullOrEmpty(product.ProductType))
            {
                parts.Add($"Loại sản phẩm: {product.ProductType}");
            }
            
            if (product.Weight.HasValue)
            {
                parts.Add($"Trọng lượng: {product.Weight.Value}kg");
            }
            
            return string.Join(". ", parts);
        }

        /// <summary>
        /// Tính cosine similarity giữa hai vectors
        /// </summary>
        private double CalculateCosineSimilarity(float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length)
            {
                return 0.0;
            }

            double dotProduct = 0.0;
            double magnitude1 = 0.0;
            double magnitude2 = 0.0;

            for (int i = 0; i < vector1.Length; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitude1 += vector1[i] * vector1[i];
                magnitude2 += vector2[i] * vector2[i];
            }

            if (magnitude1 == 0.0 || magnitude2 == 0.0)
            {
                return 0.0;
            }

            return dotProduct / (Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2));
        }

        /// <summary>
        /// Lấy embeddings cho danh sách sản phẩm đã mua
        /// </summary>
        private async Task<List<(Product product, float[] embedding)>> GetPurchasedProductsEmbeddingsAsync(List<int> productIds)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => productIds.Contains(p.ProductID))
                .ToListAsync();

            var productEmbeddings = new List<(Product product, float[] embedding)>();

            foreach (var product in products)
            {
                var embedding = await GetProductEmbeddingAsync(product);
                if (embedding != null)
                {
                    productEmbeddings.Add((product, embedding));
                }
            }

            return productEmbeddings;
        }

        /// <summary>
        /// Content-based filtering sử dụng embeddings và cosine similarity
        /// </summary>
        private async Task<List<Product>> GetContentBasedRecommendationsWithEmbeddingsAsync(
            List<(Product product, float[] embedding)> purchasedProductEmbeddings,
            List<int> purchasedProductIds,
            int count)
        {
            if (!purchasedProductEmbeddings.Any())
            {
                return new List<Product>();
            }

            try
            {
                // Tính average embedding của các sản phẩm đã mua
                var embeddingDimension = purchasedProductEmbeddings[0].embedding.Length;
                var averageEmbedding = new float[embeddingDimension];
                
                foreach (var (_, embedding) in purchasedProductEmbeddings)
                {
                    for (int i = 0; i < embeddingDimension; i++)
                    {
                        averageEmbedding[i] += embedding[i];
                    }
                }
                
                for (int i = 0; i < embeddingDimension; i++)
                {
                    averageEmbedding[i] /= purchasedProductEmbeddings.Count;
                }

                // Lấy tất cả sản phẩm chưa mua
                var candidateProducts = await _context.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.Category)
                    .Include(p => p.Inventory)
                    .Where(p => !purchasedProductIds.Contains(p.ProductID) &&
                                p.IsActive == true &&
                                (p.Inventory == null || p.Inventory.QuantityInStock > 0))
                    .ToListAsync();

                var productSimilarities = new List<(Product product, double similarity)>();

                // Tính similarity cho mỗi sản phẩm candidate
                foreach (var candidate in candidateProducts)
                {
                    var candidateEmbedding = await GetProductEmbeddingAsync(candidate);
                    if (candidateEmbedding != null)
                    {
                        var similarity = CalculateCosineSimilarity(averageEmbedding, candidateEmbedding);
                        productSimilarities.Add((candidate, similarity));
                    }
                }

                // Sắp xếp theo similarity và lấy top products
                return productSimilarities
                    .OrderByDescending(x => x.similarity)
                    .Take(count)
                    .Select(x => x.product)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in content-based recommendations with embeddings: {ex.Message}");
                return new List<Product>();
            }
        }

        /// <summary>
        /// Gợi ý sản phẩm dựa trên lịch sử mua hàng của người dùng (AI-based collaborative filtering + embeddings-based content filtering)
        /// </summary>
        public async Task<List<Product>> GetRecommendedProductsForUserAsync(int userId, int count = 8)
        {
            try
            {
                // 1. Lấy danh sách sản phẩm user đã mua
                var purchasedProductIds = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .Where(oi => oi.Order.UserID == userId && oi.Order.StatusID != 5) // Không tính đơn hàng đã hủy
                    .Select(oi => oi.ProductID)
                    .Distinct()
                    .ToListAsync();

                if (!purchasedProductIds.Any())
                {
                    // Nếu user chưa mua gì, trả về sản phẩm nổi bật
                    return await GetFeaturedProductsAsync(count);
                }

                // 2. Lấy categories của các sản phẩm đã mua
                var purchasedCategories = await _context.Products
                    .Where(p => purchasedProductIds.Contains(p.ProductID))
                    .Select(p => p.CategoryID)
                    .Distinct()
                    .ToListAsync();

                // 3. Collaborative Filtering: Tìm user khác mua sản phẩm tương tự
                var similarUserIds = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .Where(oi => purchasedProductIds.Contains(oi.ProductID) && 
                                 oi.Order.UserID != userId &&
                                 oi.Order.StatusID != 5)
                    .Select(oi => oi.Order.UserID)
                    .Distinct()
                    .Take(20) // Giới hạn số user để tối ưu hiệu suất
                    .ToListAsync();

                // 4. Lấy sản phẩm mà các user tương tự đã mua (collaborative recommendations)
                var collaborativeRecommendations = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .Include(oi => oi.Product)
                        .ThenInclude(p => p.ProductImages)
                    .Include(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                    .Include(oi => oi.Product)
                        .ThenInclude(p => p.Inventory)
                    .Where(oi => similarUserIds.Contains(oi.Order.UserID) &&
                                 !purchasedProductIds.Contains(oi.ProductID) &&
                                 oi.Product.IsActive == true &&
                                 oi.Order.StatusID != 5 &&
                                 (oi.Product.Inventory == null || oi.Product.Inventory.QuantityInStock > 0))
                    .GroupBy(oi => oi.ProductID)
                    .Select(g => new
                    {
                        Product = g.First().Product,
                        PurchaseCount = g.Count(),
                        TotalQuantity = g.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(x => x.PurchaseCount)
                    .ThenByDescending(x => x.TotalQuantity)
                    .Take(count / 2)
                    .Select(x => x.Product)
                    .ToListAsync();

                // 5. Content-based filtering với embeddings (nếu có OpenAI API key)
                List<Product> contentRecommendations;
                var useEmbeddings = _configuration.GetValue<bool>("OpenAISettings:UseEmbeddings", false);
                var apiKey = _configuration["OpenAISettings:ApiKey"];
                
                if (useEmbeddings && !string.IsNullOrEmpty(apiKey) && apiKey != "YOUR_OPENAI_API_KEY_HERE")
                {
                    // Sử dụng embeddings-based content filtering
                    var purchasedProductEmbeddings = await GetPurchasedProductsEmbeddingsAsync(purchasedProductIds);
                    
                    if (purchasedProductEmbeddings.Any())
                    {
                        // Lấy recommendations dựa trên embeddings similarity
                        contentRecommendations = await GetContentBasedRecommendationsWithEmbeddingsAsync(
                            purchasedProductEmbeddings, 
                            purchasedProductIds, 
                            count / 2);
                    }
                    else
                    {
                        // Fallback về category-based nếu không thể lấy embeddings
                        contentRecommendations = await GetCategoryBasedRecommendationsAsync(
                            purchasedCategories, 
                            purchasedProductIds, 
                            count / 2);
                    }
                }
                else
                {
                    // Sử dụng category-based filtering truyền thống
                    contentRecommendations = await GetCategoryBasedRecommendationsAsync(
                        purchasedCategories, 
                        purchasedProductIds, 
                        count / 2);
                }

                // 6. Kết hợp cả hai phương pháp
                var recommendations = collaborativeRecommendations
                    .Concat(contentRecommendations)
                    .GroupBy(p => p.ProductID)
                    .Select(g => g.First())
                    .Take(count)
                    .ToList();

                // 7. Nếu không đủ, thêm sản phẩm nổi bật
                if (recommendations.Count < count)
                {
                    var remainingCount = count - recommendations.Count;
                    var featured = await _context.Products
                        .Include(p => p.ProductImages)
                        .Include(p => p.Category)
                        .Include(p => p.Inventory)
                        .Where(p => !purchasedProductIds.Contains(p.ProductID) &&
                                    !recommendations.Select(r => r.ProductID).Contains(p.ProductID) &&
                                    p.IsActive == true &&
                                    (p.Inventory == null || p.Inventory.QuantityInStock > 0))
                        .OrderByDescending(p => p.IsFeatured)
                        .ThenBy(p => p.SalePrice != null ? p.SalePrice : p.Price)
                        .Take(remainingCount)
                        .ToListAsync();

                    recommendations.AddRange(featured);
                }

                return recommendations;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting recommended products for user {userId}: {ex.Message}");
                return await GetFeaturedProductsAsync(count);
            }
        }

        /// <summary>
        /// Content-based filtering truyền thống dựa trên category
        /// </summary>
        private async Task<List<Product>> GetCategoryBasedRecommendationsAsync(
            List<int> purchasedCategories, 
            List<int> purchasedProductIds, 
            int count)
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => purchasedCategories.Contains(p.CategoryID) &&
                            !purchasedProductIds.Contains(p.ProductID) &&
                            p.IsActive == true &&
                            (p.Inventory == null || p.Inventory.QuantityInStock > 0))
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.SalePrice != null ? (p.Price - p.SalePrice) : 0)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy sản phẩm nổi bật (fallback khi không có lịch sử)
        /// </summary>
        private async Task<List<Product>> GetFeaturedProductsAsync(int count)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.ProductImages)
                    .Include(p => p.Category)
                    .Include(p => p.Inventory)
                    .Where(p => p.IsActive == true &&
                                (p.Inventory == null || p.Inventory.QuantityInStock > 0))
                    .OrderByDescending(p => p.IsFeatured)
                    .ThenBy(p => p.SalePrice != null ? p.SalePrice : p.Price)
                    .ThenByDescending(p => p.IsNew)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting featured products: {ex.Message}");
                return new List<Product>();
            }
        }
    }

    /// <summary>
    /// Model cho phản hồi chatbot
    /// </summary>
    public class ChatbotResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<Product> SuggestedProducts { get; set; } = new List<Product>();
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
    }

    /// <summary>
    /// Model cho tin nhắn chat
    /// </summary>
    public class ChatMessage
    {
        public string Content { get; set; } = string.Empty;
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Model cho context database
    /// </summary>
    public class DatabaseContext
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
    }

    /// <summary>
    /// Model cho OpenAI API response
    /// </summary>
    public class OpenAIResponse
    {
        public List<OpenAIChoice>? choices { get; set; }
    }

    public class OpenAIChoice
    {
        public OpenAIMessage? message { get; set; }
    }

    public class OpenAIMessage
    {
        public string? content { get; set; }
    }

    /// <summary>
    /// Model cho tiêu chí tìm kiếm sản phẩm
    /// </summary>
    public class SearchCriteria
    {
        public string? PetType { get; set; } // chó, mèo
        public string? ProductType { get; set; } // thức ăn, phụ kiện, đồ chơi
        public decimal? MaxPrice { get; set; } // giá tối đa
        public int? Weight { get; set; } // trọng lượng
        public bool OnSale { get; set; } // đang khuyến mãi
    }

    /// <summary>
    /// Model cho OpenAI Embeddings API response
    /// </summary>
    public class OpenAIEmbeddingResponse
    {
        public string? @object { get; set; }
        public List<OpenAIEmbeddingData>? data { get; set; }
        public string? model { get; set; }
    }

    public class OpenAIEmbeddingData
    {
        public string? @object { get; set; }
        public float[]? embedding { get; set; }
        public int? index { get; set; }
    }
}
