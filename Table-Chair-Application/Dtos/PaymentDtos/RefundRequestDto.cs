using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.PaymentDtos
{
    public class RefundRequestDto
    {
        public string TransactionId { get; set; } = default!;

        public decimal Amount { get; set; }

        public string Reason { get; set; } = string.Empty;

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }

}
