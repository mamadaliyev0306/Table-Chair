using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos
{
    public class FaqUpdateDto
    {
        public int CategoryId { get; set; }
        public string Title { get; set; } = null!;
        public string Answer { get; set; } = string.Empty;
    }
}
