using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.UserDtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Foydalanuvchi nomi yoki email talab qilinadi")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parol majburiy")]
        [MinLength(6, ErrorMessage = "Parol kamida 6 ta belgidan iborat bo'lishi kerak")]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }=false;
    }
}
