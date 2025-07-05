using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.CreateDtos
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Kategoriya nomi majburiy")]
        [StringLength(300, MinimumLength = 2, ErrorMessage = "Kategoriya nomi 2-300 belgi oralig'ida bo'lishi kerak")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Kategoriya turi majburiy")]
        public CategoryType Type { get; set; }
    }
}
