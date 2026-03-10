using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        // 🔐 User identity
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        // 🔥 Always use UTC for backend systems
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ================= ADDRESS =================
        public int? AddressId { get; set; }

        [ForeignKey("AddressId")]
        public Address? Address { get; set; }

        // ================= ORDER STATUS =================
        // Pending → Processing → Shipped → Delivered → Cancelled
        public string Status { get; set; } = "Pending";

        // ================= ITEMS =================
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
