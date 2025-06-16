using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.PaymentDtos
{
    public class PaymentFilterDto
    {
        public int userId { get; set; }
        public int orderId { get; set; }
        public PaymentStatus? Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
