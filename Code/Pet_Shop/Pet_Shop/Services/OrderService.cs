using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;
using Pet_Shop.Models.ViewModels;
using System.Security.Claims;

namespace Pet_Shop.Services
{
    public class OrderService
    {
        private readonly PetShopDbContext _context;
        private readonly CartService _cartService;
        private readonly EmailService _emailService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(PetShopDbContext context, CartService cartService, EmailService emailService, ILogger<OrderService> logger)
        {
            _context = context;
            _cartService = cartService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Order?> CreateOrderAsync(int userId, CheckoutViewModel checkoutModel)
        {
            try
            {
                // Get cart items
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                if (!cartItems.Any())
                {
                    _logger.LogWarning($"No items in cart for user {userId}");
                    return null;
                }

                // Calculate totals
                var subtotal = cartItems.Sum(item => item.TotalPrice);
                var shippingFee = subtotal > 500000 ? 0 : 30000; // Free shipping over 500k
                var discountAmount = 0m; // TODO: Calculate from promo code
                var totalAmount = subtotal + shippingFee - discountAmount;

                // Create order
                var order = new Order
                {
                    OrderNumber = GenerateOrderNumber(),
                    UserID = userId,
                    OrderDate = DateTime.Now,
                    StatusID = 1, // New status
                    PaymentMethodID = checkoutModel.PaymentMethodId,
                    SubTotal = subtotal,
                    ShippingFee = shippingFee,
                    DiscountAmount = discountAmount,
                    TotalAmount = totalAmount,
                    ShippingAddress = FormatShippingAddress(checkoutModel),
                    Notes = checkoutModel.Notes,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create order items
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderID = order.OrderID,
                        ProductID = cartItem.ProductID,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.SalePrice ?? cartItem.Price,
                        TotalPrice = cartItem.TotalPrice
                    };
                    _context.OrderItems.Add(orderItem);
                }

                // Create order status history
                var statusHistory = new OrderStatusHistory
                {
                    OrderID = order.OrderID,
                    NewStatusID = 1, // New
                    ChangedBy = userId,
                    ChangedDate = DateTime.Now,
                    Notes = "Đơn hàng được tạo"
                };
                _context.OrderStatusHistory.Add(statusHistory);

                // Update inventory
                await UpdateInventoryAsync(cartItems);

                // Clear cart
                await _cartService.ClearCartAsync(userId);

                await _context.SaveChangesAsync();

                // Send confirmation email
                await SendOrderConfirmationEmailAsync(order);

                _logger.LogInformation($"Order {order.OrderNumber} created successfully for user {userId}");
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating order for user {userId}: {ex.Message}");
                return null;
            }
        }

        public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Status)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting order {orderNumber}: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Status)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.OrderItems)
                    .Where(o => o.UserID == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting orders for user {userId}: {ex.Message}");
                return new List<Order>();
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, int newStatusId, int changedBy, string? notes = null)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning($"Order {orderId} not found");
                    return false;
                }

                var oldStatusId = order.StatusID;
                order.StatusID = newStatusId;
                order.UpdatedDate = DateTime.Now;

                // Create status history
                var statusHistory = new OrderStatusHistory
                {
                    OrderID = orderId,
                    OldStatusID = oldStatusId,
                    NewStatusID = newStatusId,
                    ChangedBy = changedBy,
                    ChangedDate = DateTime.Now,
                    Notes = notes
                };
                _context.OrderStatusHistory.Add(statusHistory);

                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Order {orderId} status updated from {oldStatusId} to {newStatusId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating order {orderId} status: {ex.Message}");
                return false;
            }
        }

        // Admin Order Management Methods
        public async Task<IEnumerable<Order>> GetAllOrdersForAdminAsync()
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Status)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all orders for admin: {ex.Message}");
                return new List<Order>();
            }
        }

        public async Task<Order?> GetOrderByIdForAdminAsync(int orderId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Status)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.Promotion)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.ProductImages)
                    .Include(o => o.OrderStatusHistories)
                        .ThenInclude(osh => osh.OldStatus)
                    .Include(o => o.OrderStatusHistories)
                        .ThenInclude(osh => osh.NewStatus)
                    .FirstOrDefaultAsync(o => o.OrderID == orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting order {orderId} for admin: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Order>> SearchOrdersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                    return await GetAllOrdersForAdminAsync();

                return await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Status)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Where(o => o.OrderNumber.Contains(searchTerm) ||
                               o.User.FullName.Contains(searchTerm) ||
                               o.User.Email.Contains(searchTerm) ||
                               o.ShippingAddress.Contains(searchTerm))
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching orders: {ex.Message}");
                return new List<Order>();
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(int statusId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Status)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Where(o => o.StatusID == statusId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting orders by status {statusId}: {ex.Message}");
                return new List<Order>();
            }
        }

        public async Task<IEnumerable<OrderStatus>> GetAllOrderStatusesAsync()
        {
            try
            {
                return await _context.OrderStatuses
                    .OrderBy(s => s.SortOrder)
                    .ThenBy(s => s.StatusName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting order statuses: {ex.Message}");
                return new List<OrderStatus>();
            }
        }

        public async Task<bool> UpdateOrderAdminNotesAsync(int orderId, string adminNotes)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning($"Order {orderId} not found");
                    return false;
                }

                order.AdminNotes = adminNotes;
                order.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Order {orderId} admin notes updated");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating order {orderId} admin notes: {ex.Message}");
                return false;
            }
        }

        private string GenerateOrderNumber()
        {
            return "PS" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private string FormatShippingAddress(CheckoutViewModel model)
        {
            var addressParts = new List<string>();
            
            if (!string.IsNullOrEmpty(model.Address))
                addressParts.Add(model.Address);
            if (!string.IsNullOrEmpty(model.Ward))
                addressParts.Add(model.Ward);
            if (!string.IsNullOrEmpty(model.District))
                addressParts.Add(model.District);
            if (!string.IsNullOrEmpty(model.City))
                addressParts.Add(model.City);

            return string.Join(", ", addressParts);
        }

        private async Task UpdateInventoryAsync(IEnumerable<CartItemViewModel> cartItems)
        {
            try
            {
                foreach (var item in cartItems)
                {
                    var inventory = await _context.Inventory
                        .FirstOrDefaultAsync(i => i.ProductID == item.ProductID);

                    if (inventory != null)
                    {
                        inventory.QuantityInStock -= item.Quantity;
                        inventory.LastUpdated = DateTime.Now;

                        // Create inventory transaction
                        var transaction = new InventoryTransaction
                        {
                            ProductID = item.ProductID,
                            TransactionType = "Export",
                            Quantity = item.Quantity,
                            UnitPrice = item.SalePrice ?? item.Price,
                            TotalValue = item.TotalPrice,
                            ReferenceNumber = "ORDER_" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                            Notes = "Bán hàng",
                            CreatedBy = 1, // TODO: Get actual user ID
                            CreatedDate = DateTime.Now
                        };
                        _context.InventoryTransactions.Add(transaction);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating inventory: {ex.Message}");
            }
        }

        private async Task SendOrderConfirmationEmailAsync(Order order)
        {
            try
            {
                var user = await _context.Users.FindAsync(order.UserID);
                if (user == null) return;

                var subject = $"Xác nhận đơn hàng #{order.OrderNumber} - HyHy Pet Shop";
                var body = $@"
                    <h2>Xác nhận đơn hàng #{order.OrderNumber}</h2>
                    <p>Xin chào {user.FullName},</p>
                    <p>Cảm ơn bạn đã đặt hàng tại HyHy Pet Shop!</p>
                    <p><strong>Thông tin đơn hàng:</strong></p>
                    <ul>
                        <li>Mã đơn hàng: {order.OrderNumber}</li>
                        <li>Ngày đặt: {order.OrderDate:dd/MM/yyyy HH:mm}</li>
                        <li>Tổng tiền: {order.TotalAmount:N0} ₫</li>
                    </ul>
                    <p>Chúng tôi sẽ xử lý đơn hàng của bạn trong thời gian sớm nhất.</p>
                ";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending order confirmation email: {ex.Message}");
            }
        }
    }
}
