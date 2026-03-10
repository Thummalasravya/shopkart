namespace ECommerceAPI.DTOs
{
    public class CategoryDto
    {
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public string Description { get; set; } = "";
        public int? ParentId { get; set; }
        public string ImageUrl { get; set; } = "";
    }
}
