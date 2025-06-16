using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.PaymentDtos
{
    public class PaymentStatusDto
    {
        public PaymentStatus Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? TransactionId { get; set; }
    }
}
