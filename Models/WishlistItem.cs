namespace ECommerceAPI.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
         // 🔐 USER IDENTIFIER
    public string UserEmail { get; set; } = "";
    }
}
