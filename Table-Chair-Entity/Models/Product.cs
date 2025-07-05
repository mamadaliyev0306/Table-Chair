using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Sofdelet;

namespace Table_Chair_Entity.Models
{
    [Table("Product",Schema ="Models")]
    public class Product:ISoftDeletable
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;
        [MaxLength(500)] public string? Description { get; set; }
        [Required]
        [Range(0.01,double.MaxValue)]
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int? UserId { get; set; }
  
        public User? User { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; } = new LinkedList<CartItem>();
        public virtual ICollection<WishlistItem> WishlistItems { get; set; }= new HashSet<WishlistItem>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        [Range(0,double.MaxValue)]
        public int StockQuantity { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Range(0, 100)]
        public int DiscountPercent { get; set; } = 0;  // 0-100% oralig'ida chegirma

        [Range(0, 5)]
        public double AverageRating { get; set; } = 0;  // 0-5 oralig'ida reyting
        public DateTime? DeletedAt { get; set; }
        public int TotalOrders { get; set; } = 0;  // Umumiy buyurtmalar soni

        // Yangi hisoblangan narx (readonly property)
        [NotMapped]  // Ma'lumotlar bazasiga saqlanmaydi
        public decimal DiscountedPrice => Price * (100 - DiscountPercent) / 100;
    }
}
