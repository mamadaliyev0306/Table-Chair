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
    [Table("OrderStatusHistories", Schema = "ordering")]
    public class OrderStatusHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public virtual Order? Order { get; set; } 

        [Required, MaxLength(50)]
        public OrderStatus Status { get; set; }=OrderStatus.Created;

        [Required]
        public DateTime ChangedAt { get; set; } 

        [MaxLength(100)]
        public string? ChangedBy { get; set; } 

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
