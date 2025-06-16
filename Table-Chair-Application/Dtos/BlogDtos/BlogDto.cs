using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.BlogDtos
{
    public class BlogDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; // Category nomi, agar kerak bo'lsa

        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
    }
}
