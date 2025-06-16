using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.AboutInfoDtos
{
    public class AboutInfoUpdateDto
    {
        [Required] public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
