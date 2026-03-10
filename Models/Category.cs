using System.Collections.Generic;

namespace ECommerceAPI.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public string Description { get; set; } = "";

        // NULL = root category
        public int? ParentId { get; set; }
        public Category? Parent { get; set; }

        public string ImageUrl { get; set; } = "";

        public ICollection<Category> Children { get; set; } = new List<Category>();
    }
}
