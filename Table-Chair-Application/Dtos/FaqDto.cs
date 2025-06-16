using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos
{
    public class FaqDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string Title { get; set; } = null!;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public string Answer { get; set; } = string.Empty;
    }
}
