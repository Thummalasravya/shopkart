using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Navigation
        public Order? Order { get; set; }
        public Product ?Product { get; set; }
    }
}
