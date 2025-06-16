using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.PaymentDtos
{
    public class PaginatedPaymentResponseDto
    {
        public IEnumerable<PaymentResponseDto> Payments { get; set; } = null!;
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
