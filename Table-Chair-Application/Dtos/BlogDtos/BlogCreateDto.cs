using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.BlogDtos
{
    public class BlogCreateDto
    {
        public int CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
