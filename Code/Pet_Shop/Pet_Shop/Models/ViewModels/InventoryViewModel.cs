using System.ComponentModel.DataAnnotations;

namespace Pet_Shop.Models.ViewModels
{
    public class InventoryViewModel
    {
        public int InventoryID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }
        public int MinStockLevel { get; set; }
        public int MaxStockLevel { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsLowStock { get; set; }
        public bool IsOutOfStock { get; set; }
    }

    public class InventoryTransactionViewModel
    {
        public int TransactionID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string TransactionTypeName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalValue { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class InventoryStatisticsViewModel
    {
        public int TotalProducts { get; set; }
        public decimal TotalStockValue { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int OverStockProducts { get; set; }
        public int TotalImports30Days { get; set; }
        public int TotalExports30Days { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class InventoryTransactionFormViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại giao dịch")]
        public string TransactionType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
        public decimal? UnitPrice { get; set; }

        [StringLength(50, ErrorMessage = "Số phiếu không được vượt quá 50 ký tự")]
        public string? ReferenceNumber { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }

        // Thông tin sản phẩm để hiển thị
        public string? ProductName { get; set; }
        public int? CurrentStock { get; set; }
        public int? MinStockLevel { get; set; }
        public int? MaxStockLevel { get; set; }
    }
}
