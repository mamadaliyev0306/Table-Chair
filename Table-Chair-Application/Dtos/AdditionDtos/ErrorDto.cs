using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.AdditionDtos
{
    public class ErrorDto
    {
        public string? Message { get; set; }
        public string? Details { get; set; }
        public string? StackTrace { get; set; }
    }
}
