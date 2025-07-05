using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.UserDtos
{
    using System.ComponentModel.DataAnnotations;

    public class UserUpdateDto
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username 3-50 belgidan iborat bo'lishi kerak!")]
        public string? Username { get; set; }

        [StringLength(100, ErrorMessage = "Ism 100 belgidan oshmasligi kerak!")]
        public string? FirstName { get; set; }

        [StringLength(100, ErrorMessage = "Familiya 100 belgidan oshmasligi kerak!")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Telefon raqam formati noto'g'ri!")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email formati noto'g'ri!")]
        public string? Email { get; set; }

        [Url(ErrorMessage = "Profil rasmi URL formati noto'g'ri!")]
        public string? AvatarUrl { get; set; }

        [StringLength(500, ErrorMessage = "Bio 500 belgidan oshmasligi kerak!")]
        public string? Bio { get; set; }
    }
}
