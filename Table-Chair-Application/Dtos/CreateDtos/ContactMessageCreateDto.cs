using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.CreateDtos
{
    public class ContactMessageCreateDto
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; } 

        [Required, EmailAddress]
        public string Email { get; set; }=null!;

        [Required, MaxLength(1000)]
        public string Message { get; set; } = null!;
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
