using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos
{
    public  class OrderUpdateDto
    {
        public int Id { get; set; }
        public int? ShippingAddressId { get; set; }
        public OrderStatus? Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
    }
}
