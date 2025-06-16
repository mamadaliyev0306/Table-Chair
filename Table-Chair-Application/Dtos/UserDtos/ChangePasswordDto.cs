using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.UserDtos
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Joriy parol majburiy")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yangi parol majburiy")]
        [MinLength(6, ErrorMessage = "Parol kamida 6 ta belgidan iborat bo'lishi kerak")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Parollar mos kelmayapti")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
