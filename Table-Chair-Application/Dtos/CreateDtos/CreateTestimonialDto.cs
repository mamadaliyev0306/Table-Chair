using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.CreateDtos
{
    public class CreateTestimonialDto
    {
        [Required]
        [MaxLength(100)]
        public string AuthorName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
    }
}
