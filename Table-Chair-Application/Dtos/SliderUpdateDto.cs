using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos
{
    public class SliderUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        public string? ImageUrl { get; set; }

        [MaxLength(150)]
        public string? Title { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        public string? RedirectUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
}
