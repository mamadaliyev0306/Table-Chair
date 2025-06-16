using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.UserDtos
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email majburiy")]
        [EmailAddress(ErrorMessage = "Noto'g'ri email formati")]
        public string Email { get; set; } = string.Empty;
    }
}
