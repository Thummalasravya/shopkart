using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal Rating { get; set; }

        public string Brand { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public int StockQuantity { get; set; }

        public int Stock { get; set; }

        public bool Availability { get; set; }   

        public bool IsEnabled { get; set; }

        public bool IsExchangeable { get; set; }

        public bool IsRefundable { get; set; }

        public int Sold { get; set; }

        public DateTime CreatedAt { get; set; }

        // ✅ Required Navigation Properties
        public Category? CategoryRef { get; set; }

        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}