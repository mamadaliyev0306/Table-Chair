using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos
{
    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "ID majburiy")]
        public int Id { get; set; }

        [StringLength(300, MinimumLength = 2, ErrorMessage = "Kategoriya nomi 2-300 belgi oralig'ida bo'lishi kerak")]
        public string? Name { get; set; }

        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
