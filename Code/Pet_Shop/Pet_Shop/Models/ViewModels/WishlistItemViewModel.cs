namespace Pet_Shop.Models.ViewModels
{
    public class WishlistItemViewModel
    {
        public int WishlistID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int StockQuantity { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
