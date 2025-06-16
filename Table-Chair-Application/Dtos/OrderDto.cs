using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; } = null!;
        public int ShippingAddressId { get; set; }
        public string ShippingAddressText { get; set; } = null!;
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
        public List<PaymentResponseDto> Payments { get; set; } = new();
        public List<OrderStatusHistoryDto> StatusHistories { get; set; } = new();
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CancelledAt { get; set; }
        public decimal TotalAmount { get; set; }
    }


}
