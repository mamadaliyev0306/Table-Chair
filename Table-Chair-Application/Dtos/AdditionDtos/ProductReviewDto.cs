using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.AdditionDtos
{
    public class ProductReviewDto
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; } // 1 - 5
        public DateTime CreatedAt { get; set; }
    }
}
