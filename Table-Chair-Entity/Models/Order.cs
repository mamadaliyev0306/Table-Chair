using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Entity.Models
{
    [Table("Orders", Schema = "ordering")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        [Required]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
        [Required]
        public int ShippingAddressId { get; set; }

        [ForeignKey(nameof(ShippingAddressId))]
        public virtual ShippingAddress ShippingAddress { get; set; } = null!;

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Created;
        [Required]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new LinkedList<OrderItem>();
        public virtual ICollection<Payment> Payments { get; set; } =new LinkedList<Payment>();
        public virtual ICollection<OrderStatusHistory> StatusHistories { get; set; } = new HashSet<OrderStatusHistory>();
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }=false;
        public DateTime? CancelledAt { get; set; }
        public decimal TotalAmount { get; set; } 
    }
}
