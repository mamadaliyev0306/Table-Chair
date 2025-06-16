using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.AdditionDtos
{
    public class OrderItemFilterDto
    {
        public int? UserId { get; set; }
        public OrderStatus? Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        // Sana oralig‘i bo‘yicha filter
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Buyurtma umumiy summasi oralig‘i bo‘yicha filter
        public decimal? MinTotalAmount { get; set; }
        public decimal? MaxTotalAmount { get; set; }

        // O‘chirilgan/o‘chirilmagan holat
        public bool? IsDeleted { get; set; }
    }
}
