namespace Pet_Shop.Models.ViewModels
{
    public class CartItemViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => (SalePrice ?? Price) * Quantity;
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public int StockQuantity { get; set; }
    }
}
