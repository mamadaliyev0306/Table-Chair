using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Dtos.DetailsDtos
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }

        // Foydalanuvchi ma'lumotlari
        public int UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }

        // Yetkazib berish manzili
        public ShippingAddress ShippingAddress { get; set; } = null!;

        // Buyurtma elementlari
        public List<OrderItemDetailsDto> OrderItemDetailsDtos { get; set; }= null!;

        // To'lov ma'lumotlari
        public List<PaymentDetailsDto> Payments { get; set; } =null!;

        // Buyurtma tarixi (status o'zgarishlari)
        public List<OrderStatusHistoryDto> StatusHistory { get; set; } = null!;
    }
}
