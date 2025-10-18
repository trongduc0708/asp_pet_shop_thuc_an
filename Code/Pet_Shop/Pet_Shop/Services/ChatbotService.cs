using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;
using System.Text.Json;
using System.Text;

namespace Pet_Shop.Services
{
    public class ChatbotService
    {
        private readonly PetShopDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly HttpClient _httpClient;

        public ChatbotService(PetShopDbContext context, IConfiguration configuration, ProductService productService, CategoryService categoryService, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _productService = productService;
            _categoryService = categoryService;
            _httpClient = httpClient;
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

            return $@"Bạn là trợ lý AI thông minh của HyHy Pet Shop - cửa hàng chuyên cung cấp thức ăn và phụ kiện cho thú cưng.

THÔNG TIN CỬA HÀNG:
- Tên: HyHy Pet Shop
- Chuyên: Thức ăn và phụ kiện cho chó, mèo
- Sản phẩm chất lượng cao từ các thương hiệu uy tín

DANH SÁCH SẢN PHẨM HIỆN CÓ (với thông tin chi tiết):
{productList}

DANH MỤC SẢN PHẨM:
{categoryList}

THƯƠNG HIỆU:
{brandList}

NHIỆM VỤ CỦA BẠN:
1. Tư vấn sản phẩm phù hợp dựa trên nhu cầu của khách hàng
2. Gợi ý sản phẩm theo loại thú cưng (chó, mèo)
3. Tư vấn về dinh dưỡng và chăm sóc thú cưng
4. Hỗ trợ tìm kiếm sản phẩm theo giá cả, thương hiệu, trọng lượng
5. Luôn thân thiện, chuyên nghiệp và hữu ích

QUY TẮC TRẢ LỜI QUAN TRỌNG:
- Trả lời bằng tiếng Việt
- Thân thiện và chuyên nghiệp
- CHỈ gợi ý sản phẩm khi khách hàng thực sự hỏi về sản phẩm
- KHÔNG gợi ý sản phẩm khi khách hàng chỉ chào hỏi hoặc hỏi chung
- Khi gợi ý sản phẩm, đề xuất sản phẩm cụ thể với ID và tên
- Giải thích lý do đề xuất dựa trên thông tin sản phẩm thực tế
- Hỏi thêm thông tin nếu cần thiết

CÁC TRƯỜNG HỢP KHÔNG GỢI Ý SẢN PHẨM:
- Chào hỏi: xin chào, hello, hi
- Câu hỏi chung: bạn là ai, bạn có thể làm gì
- Hỏi thông tin: giờ mở cửa, địa chỉ, liên hệ

CÁC TRƯỜNG HỢP NÊN GỢI Ý SẢN PHẨM:
- Hỏi về sản phẩm: tôi cần thức ăn cho chó, sản phẩm nào cho mèo
- Tìm kiếm: tìm phụ kiện, đồ chơi cho chó
- Giá cả: sản phẩm nào rẻ, khuyến mãi gì
- Yêu cầu cụ thể: thức ăn cho chó dưới 200k, phụ kiện cho mèo 3kg

Khi đề xuất sản phẩm, hãy đề cập đến ID sản phẩm để hệ thống có thể hiển thị chi tiết.
Sử dụng thông tin sản phẩm thực tế từ database để đưa ra gợi ý chính xác và phù hợp.";
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
}
