# Chatbot AI với OpenAI Integration

## Tổng Quan
Hệ thống chatbot AI thông minh được tích hợp vào HyHy Pet Shop, sử dụng OpenAI API để tư vấn sản phẩm và hỗ trợ khách hàng.

## Tính Năng Chính

### 1. **AI Chatbot Thông Minh**
- Tích hợp OpenAI GPT-3.5-turbo
- Hiểu ngữ cảnh và trả lời tự nhiên
- Tư vấn sản phẩm dựa trên database
- Gợi ý sản phẩm phù hợp

### 2. **Giao Diện Hiện Đại**
- Widget chatbot floating button
- Giao diện chat thân thiện
- Animation mượt mà
- Responsive trên mọi thiết bị

### 3. **Tính Năng Tương Tác**
- Chat real-time với AI
- Gợi ý sản phẩm trực tiếp
- Quick actions cho câu hỏi thường gặp
- Lịch sử cuộc trò chuyện

### 4. **Fallback System**
- Logic thông minh khi không có OpenAI API
- Tìm kiếm sản phẩm dựa trên từ khóa
- Phân loại tự động theo loại thú cưng

## Cấu Trúc Code

### 1. **Backend Services**
- `ChatbotService.cs`: Service xử lý AI và database
- `ChatbotController.cs`: API endpoints
- `Program.cs`: Đăng ký services

### 2. **Frontend**
- `_Layout.cshtml`: Widget chatbot
- `site.css`: CSS cho chatbot (dòng 729-1171)
- JavaScript: Logic tương tác

### 3. **Database Integration**
- Đọc sản phẩm từ database
- Tìm kiếm thông minh
- Gợi ý sản phẩm phù hợp

## Cài Đặt

### 1. **Cấu Hình OpenAI API Key**
```json
// appsettings.json
{
  "OpenAISettings": {
    "ApiKey": "YOUR_OPENAI_API_KEY_HERE",
    "Model": "gpt-3.5-turbo",
    "MaxTokens": 1000,
    "Temperature": 0.7
  }
}
```

### 2. **Lấy OpenAI API Key**
1. Truy cập [OpenAI Platform](https://platform.openai.com/)
2. Đăng ký/đăng nhập tài khoản
3. Tạo API key mới
4. Copy API key vào `appsettings.json`

### 3. **Fallback Mode**
Nếu không có OpenAI API key, hệ thống sẽ sử dụng logic fallback:
- Tìm kiếm sản phẩm theo từ khóa
- Phân loại theo loại thú cưng
- Gợi ý sản phẩm phù hợp

## Sử Dụng

### 1. **Khách Hàng**
- Click vào button chatbot (góc phải dưới)
- Nhập câu hỏi hoặc sử dụng quick actions
- Nhận gợi ý sản phẩm từ AI
- Click vào sản phẩm để xem chi tiết

### 2. **Quick Actions**
- 🐕 Thức ăn chó
- 🐱 Thức ăn mèo  
- 🎾 Phụ kiện
- 💰 Khuyến mãi

### 3. **Ví Dụ Câu Hỏi**
```
"Tôi cần thức ăn cho chó con"
"Sản phẩm nào đang giảm giá?"
"Phụ kiện nào tốt cho mèo?"
"Thức ăn khô cho chó trưởng thành"
```

## API Endpoints

### 1. **Gửi Tin Nhắn**
```
POST /api/Chatbot/send-message
Content-Type: application/json

{
  "message": "Tôi cần thức ăn cho chó",
  "conversationHistory": "[{\"content\":\"Xin chào\",\"isUser\":true}]"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Tôi tìm thấy một số sản phẩm dành cho chó...",
  "suggestedProducts": [
    {
      "productId": 1,
      "productName": "Royal Canin Adult",
      "price": 400000,
      "salePrice": null,
      "shortDescription": "Thức ăn cao cấp cho chó trưởng thành",
      "categoryName": "Thức ăn chó",
      "imageUrl": "/images/royal-canin.jpg",
      "altText": "Royal Canin Adult"
    }
  ]
}
```

### 2. **Tìm Kiếm Sản Phẩm**
```
GET /api/Chatbot/search-products?query=thức ăn chó
```

### 3. **Sản Phẩm Phổ Biến**
```
GET /api/Chatbot/popular-products
```

### 4. **Thông Tin Chatbot**
```
GET /api/Chatbot/info
```

## Tùy Chỉnh

### 1. **Thay Đổi Giao Diện**
Chỉnh sửa CSS trong `site.css` (dòng 729-1171):
```css
/* Thay đổi màu sắc */
.chatbot-toggle {
    background: linear-gradient(135deg, #your-color-1, #your-color-2);
}

/* Thay đổi kích thước */
.chatbot-container {
    width: 400px; /* Thay đổi width */
    height: 600px; /* Thay đổi height */
}
```

### 2. **Thêm Quick Actions**
Trong `_Layout.cshtml`:
```html
<button class="chatbot-quick-action" onclick="sendQuickMessage('Câu hỏi mới')">
    🆕 Quick Action Mới
</button>
```

### 3. **Tùy Chỉnh Logic Fallback**
Trong `ChatbotService.cs`, method `ProcessMessageWithFallbackAsync`:
```csharp
// Thêm logic mới
else if (message.Contains("từ khóa mới"))
{
    // Logic xử lý mới
}
```

### 4. **Thay Đổi System Prompt**
Trong `ChatbotService.cs`, method `CreateSystemPrompt`:
```csharp
return $@"Bạn là trợ lý AI của {shopName}...
// Thêm thông tin mới về shop
";
```

## Troubleshooting

### 1. **Chatbot Không Hiển Thị**
- Kiểm tra CSS có load không
- Kiểm tra JavaScript console có lỗi không
- Kiểm tra `_Layout.cshtml` có include chatbot không

### 2. **API Không Hoạt Động**
- Kiểm tra OpenAI API key
- Kiểm tra kết nối internet
- Kiểm tra logs trong console

### 3. **Fallback Không Hoạt Động**
- Kiểm tra database connection
- Kiểm tra `ChatbotService` có được đăng ký không
- Kiểm tra `HttpClient` có được inject không

### 4. **Sản Phẩm Không Hiển Thị**
- Kiểm tra database có dữ liệu không
- Kiểm tra `ProductService` có hoạt động không
- Kiểm tra image URLs có hợp lệ không

## Performance

### 1. **Tối Ưu Hóa**
- Sử dụng caching cho database queries
- Giới hạn số lượng sản phẩm trả về
- Lazy loading cho images

### 2. **Monitoring**
- Log API calls
- Monitor response times
- Track user interactions

## Security

### 1. **API Key Security**
- Không commit API key vào git
- Sử dụng environment variables
- Rotate API keys định kỳ

### 2. **Input Validation**
- Validate user input
- Sanitize messages
- Rate limiting

## Kết Luận

Chatbot AI giúp tăng trải nghiệm khách hàng và hiệu quả bán hàng. Hệ thống có fallback thông minh đảm bảo hoạt động ổn định ngay cả khi không có OpenAI API.

**Lưu ý**: Để sử dụng đầy đủ tính năng AI, cần cấu hình OpenAI API key. Nếu không có, hệ thống vẫn hoạt động với logic fallback thông minh.
