using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;

namespace Pet_Shop.Services
{
    public class PromotionService
    {
        private readonly PetShopDbContext _context;

        public PromotionService(PetShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách mã khuyến mãi đang hoạt động
        /// </summary>
        /// <returns>Danh sách mã khuyến mãi đang hoạt động</returns>
        public async Task<List<Promotion>> GetActivePromotionsAsync()
        {
            var currentDate = DateTime.Now;
            
            return await _context.Promotions
                .Where(p => p.IsActive && 
                           p.StartDate <= currentDate && 
                           p.EndDate >= currentDate &&
                           (p.UsageLimit == null || p.UsedCount < p.UsageLimit))
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy mã khuyến mãi theo mã code
        /// </summary>
        /// <param name="promotionCode">Mã khuyến mãi</param>
        /// <returns>Mã khuyến mãi hoặc null</returns>
        public async Task<Promotion?> GetPromotionByCodeAsync(string promotionCode)
        {
            var currentDate = DateTime.Now;
            
            return await _context.Promotions
                .FirstOrDefaultAsync(p => p.PromotionCode == promotionCode &&
                                        p.IsActive && 
                                        p.StartDate <= currentDate && 
                                        p.EndDate >= currentDate &&
                                        (p.UsageLimit == null || p.UsedCount < p.UsageLimit));
        }

        /// <summary>
        /// Tính toán số tiền giảm giá
        /// </summary>
        /// <param name="promotion">Mã khuyến mãi</param>
        /// <param name="orderAmount">Tổng tiền đơn hàng</param>
        /// <returns>Số tiền giảm giá</returns>
        public decimal CalculateDiscountAmount(Promotion promotion, decimal orderAmount)
        {
            // Kiểm tra đơn hàng tối thiểu
            if (orderAmount < promotion.MinOrderAmount)
            {
                return 0;
            }

            decimal discountAmount = 0;

            if (promotion.DiscountType == "Percentage")
            {
                discountAmount = orderAmount * (promotion.DiscountValue / 100);
            }
            else if (promotion.DiscountType == "FixedAmount")
            {
                discountAmount = promotion.DiscountValue;
            }

            // Áp dụng giới hạn giảm giá tối đa
            if (promotion.MaxDiscountAmount.HasValue && discountAmount > promotion.MaxDiscountAmount.Value)
            {
                discountAmount = promotion.MaxDiscountAmount.Value;
            }

            return discountAmount;
        }

        /// <summary>
        /// Cập nhật số lần sử dụng mã khuyến mãi
        /// </summary>
        /// <param name="promotionId">ID mã khuyến mãi</param>
        /// <returns>True nếu cập nhật thành công</returns>
        public async Task<bool> IncrementUsageCountAsync(int promotionId)
        {
            try
            {
                var promotion = await _context.Promotions.FindAsync(promotionId);
                if (promotion != null)
                {
                    promotion.UsedCount++;
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

        /// <summary>
        /// Kiểm tra mã khuyến mãi có hợp lệ không
        /// </summary>
        /// <param name="promotionCode">Mã khuyến mãi</param>
        /// <param name="orderAmount">Tổng tiền đơn hàng</param>
        /// <returns>Thông báo lỗi hoặc null nếu hợp lệ</returns>
        public async Task<string?> ValidatePromotionAsync(string promotionCode, decimal orderAmount)
        {
            var promotion = await GetPromotionByCodeAsync(promotionCode);
            
            if (promotion == null)
            {
                return "Mã khuyến mãi không tồn tại hoặc đã hết hạn";
            }

            if (orderAmount < promotion.MinOrderAmount)
            {
                return $"Đơn hàng tối thiểu {promotion.MinOrderAmount:N0} ₫ để sử dụng mã này";
            }

            if (promotion.UsageLimit.HasValue && promotion.UsedCount >= promotion.UsageLimit.Value)
            {
                return "Mã khuyến mãi đã hết lượt sử dụng";
            }

            return null; // Hợp lệ
        }
    }
}
