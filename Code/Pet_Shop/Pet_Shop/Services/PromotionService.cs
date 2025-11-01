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

        // ========== ADMIN METHODS ==========

        /// <summary>
        /// Lấy tất cả mã khuyến mãi cho admin
        /// </summary>
        /// <returns>Danh sách tất cả mã khuyến mãi</returns>
        public async Task<List<Promotion>> GetAllPromotionsForAdminAsync()
        {
            return await _context.Promotions
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy mã khuyến mãi theo ID
        /// </summary>
        /// <param name="id">ID mã khuyến mãi</param>
        /// <returns>Mã khuyến mãi hoặc null</returns>
        public async Task<Promotion?> GetPromotionByIdAsync(int id)
        {
            return await _context.Promotions.FindAsync(id);
        }

        /// <summary>
        /// Tạo mã khuyến mãi mới
        /// </summary>
        /// <param name="promotion">Mã khuyến mãi</param>
        /// <returns>True nếu tạo thành công</returns>
        public async Task<bool> CreatePromotionAsync(Promotion promotion)
        {
            try
            {
                promotion.CreatedDate = DateTime.Now;
                _context.Promotions.Add(promotion);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Cập nhật mã khuyến mãi
        /// </summary>
        /// <param name="promotion">Mã khuyến mãi</param>
        /// <returns>True nếu cập nhật thành công</returns>
        public async Task<bool> UpdatePromotionAsync(Promotion promotion)
        {
            try
            {
                _context.Promotions.Update(promotion);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Xóa mã khuyến mãi
        /// </summary>
        /// <param name="id">ID mã khuyến mãi</param>
        /// <returns>True nếu xóa thành công</returns>
        public async Task<bool> DeletePromotionAsync(int id)
        {
            try
            {
                var promotion = await _context.Promotions.FindAsync(id);
                if (promotion != null)
                {
                    _context.Promotions.Remove(promotion);
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
        /// Kiểm tra mã khuyến mãi đã tồn tại chưa
        /// </summary>
        /// <param name="promotionCode">Mã khuyến mãi</param>
        /// <param name="excludeId">ID mã khuyến mãi cần loại trừ (khi cập nhật)</param>
        /// <returns>True nếu đã tồn tại</returns>
        public async Task<bool> PromotionCodeExistsAsync(string promotionCode, int? excludeId = null)
        {
            var query = _context.Promotions.Where(p => p.PromotionCode == promotionCode);
            
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.PromotionID != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        /// <summary>
        /// Tìm kiếm mã khuyến mãi
        /// </summary>
        /// <param name="searchTerm">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách mã khuyến mãi</returns>
        public async Task<List<Promotion>> SearchPromotionsAsync(string searchTerm)
        {
            return await _context.Promotions
                .Where(p => p.PromotionCode.Contains(searchTerm) || 
                           p.PromotionName.Contains(searchTerm) ||
                           (p.Description != null && p.Description.Contains(searchTerm)))
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        /// <summary>
        /// Lọc mã khuyến mãi theo trạng thái
        /// </summary>
        /// <param name="isActive">Trạng thái hoạt động</param>
        /// <returns>Danh sách mã khuyến mãi</returns>
        public async Task<List<Promotion>> GetPromotionsByStatusAsync(bool isActive)
        {
            return await _context.Promotions
                .Where(p => p.IsActive == isActive)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy thống kê mã khuyến mãi
        /// </summary>
        /// <returns>Thống kê</returns>
        public async Task<object> GetPromotionStatisticsAsync()
        {
            var total = await _context.Promotions.CountAsync();
            var active = await _context.Promotions.CountAsync(p => p.IsActive);
            var expired = await _context.Promotions.CountAsync(p => p.EndDate < DateTime.Now);
            var usedUp = await _context.Promotions.CountAsync(p => p.UsageLimit.HasValue && p.UsedCount >= p.UsageLimit.Value);

            return new
            {
                Total = total,
                Active = active,
                Expired = expired,
                UsedUp = usedUp,
                Inactive = total - active
            };
        }
    }
}
