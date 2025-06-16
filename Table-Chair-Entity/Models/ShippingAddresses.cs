using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Entity.Models
{
    [Table("ShippingAddresses", Schema = "Models")]
    public class ShippingAddress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Phone number is invalid.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string AddressLine { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        [Required]
        public string PostalCode { get; set; } = string.Empty;

        public CountryMethod Country { get; set; } = CountryMethod.Uzbekistan;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }

}
