namespace ECommerceAPI.DTOs
{
    public class CartDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal GrandTotal => Items.Sum(i => i.Total);
    }
}
