using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.AdditionDtos
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public DateTime OrderedAt { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
