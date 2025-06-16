using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int CartId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal UnitPrice { get; internal set; }
        public string ProductName { get; internal set; } = null!;
        public string ProductImageUrl { get; internal set; } = null!;
        public bool IsAvailable { get; set; }
        public string AvailabilityMessage { get; set; } = null!;
        public decimal TotalPrice { get; internal set; }
        public bool IsDeleted { get; set; }
    }
}
