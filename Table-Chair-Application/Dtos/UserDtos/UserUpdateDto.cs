using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.UserDtos
{
    public class UserUpdateDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ism 2-100 belgi oralig'ida bo'lishi kerak")]
        public string? FirstName { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Familiya 2-100 belgi oralig'ida bo'lishi kerak")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Noto'g'ri telefon raqam formati")]
        [StringLength(20, ErrorMessage = "Telefon raqam 20 belgidan oshmasligi kerak")]
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
    }
}
