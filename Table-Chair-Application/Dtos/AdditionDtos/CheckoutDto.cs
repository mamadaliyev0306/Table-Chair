using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos.ShippingAddressDtos;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.AdditionDtos
{
    public class CheckoutDto
    {
        public int ShippingAddressId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }

}
