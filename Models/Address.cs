using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class Address
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string AddressTitle { get; set; } = "";

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = "";

        [Required]
        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; } = "";

        [Required]
        [MaxLength(500)]
        public string AddressLine { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = "";

        [Required]
        [RegularExpression(@"^\d{6}$",
            ErrorMessage = "Pincode must be exactly 6 digits")]
        public string Pincode { get; set; } = "";

        public bool IsDefault { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
