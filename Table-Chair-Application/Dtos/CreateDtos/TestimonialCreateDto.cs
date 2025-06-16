using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.CreateDtos
{
    public class TestimonialCreateDto
    {
        public string Name { get; set; }=string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
