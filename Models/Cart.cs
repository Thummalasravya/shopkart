using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public bool IsWishlisted { get; set; }

        public bool IsSelected { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        /* ================= NAVIGATION ================= */

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}