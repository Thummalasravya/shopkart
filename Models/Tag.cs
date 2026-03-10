namespace ECommerceAPI.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string Value { get; set; } = "";
    }
}
