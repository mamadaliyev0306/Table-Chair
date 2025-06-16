using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.UserDtos
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Foydalanuvchi nomi talab qilinadi")]
        [MinLength(3, ErrorMessage = "Foydalanuvchi nomi kamida 3 ta belgidan iborat bo'lishi kerak")]
        [MaxLength(50, ErrorMessage = "Foydalanuvchi nomi 50 ta belgidan oshmasligi kerak")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Faqat harflar, raqamlar va pastki chiziq (_) ruxsat etiladi")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "To'liq ism majburiy")]
        [MaxLength(100, ErrorMessage = "To'liq ism 100 ta belgidan oshmasligi kerak")]
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Telefon raqam majburiy")]
        [Phone(ErrorMessage = "Noto'g'ri telefon raqam formati")]
        [MaxLength(20, ErrorMessage = "Telefon raqam 20 ta belgidan oshmasligi kerak")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email majburiy")]
        [EmailAddress(ErrorMessage = "Noto'g'ri email formati")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parol majburiy")]
        [MinLength(6, ErrorMessage = "Parol kamida 6 ta belgidan iborat bo'lishi kerak")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Parollar mos kelmayapti")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
