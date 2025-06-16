using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Entity.Models
{
    [Table("OrderItems", Schema = "ordering")]
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public virtual Order? Order { get; set; } 
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;
        [Required]
        public int Quantity { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        public decimal PurchasedPrice { get; set; }  // Buyurtma vaqtidagi narx
        public int DiscountPercent { get; set; } = 0;  // Buyurtma vaqtidagi chegirma
        [NotMapped]
        public decimal TotalPrice => Quantity * UnitPrice;
    }
 }
