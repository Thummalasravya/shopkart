namespace ECommerceAPI.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }   // PK

        // 🔑 USER
        public int UserId { get; set; }

        // 🔗 CART (optional grouping)
        public int CartId { get; set; }
        public Cart? Cart { get; set; }

        // 📦 PRODUCT
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        // 🧮 QUANTITY
        public int Quantity { get; set; }

        // 💰 PRICE (snapshot)
        public decimal Price { get; set; }

        // ✅ CHECKOUT FLAGS
        public bool IsSelected { get; set; } = true;

        // 🕒 AUDIT
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
