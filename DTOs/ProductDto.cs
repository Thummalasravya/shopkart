namespace ECommerceAPI.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        // ⭐ Category name for frontend display
        public string Category { get; set; } = "";

        public string Brand { get; set; } = "";

        public string ImageUrl { get; set; } = "";

        public int StockQuantity { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsExchangeable { get; set; }

        public bool IsRefundable { get; set; }

        // ⭐ Avoid null warnings
        public List<string> Tags { get; set; } = new List<string>();
    }
}