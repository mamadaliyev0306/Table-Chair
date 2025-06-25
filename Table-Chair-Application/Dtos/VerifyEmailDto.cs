using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos
{
    public class VerifyEmailDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required, StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = null!;
    }
}
