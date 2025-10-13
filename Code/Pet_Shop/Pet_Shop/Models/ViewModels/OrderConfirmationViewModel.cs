namespace Pet_Shop.Models.ViewModels
{
    public class OrderConfirmationViewModel
    {
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime EstimatedDelivery { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
