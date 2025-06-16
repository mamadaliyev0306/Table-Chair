using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.AdditionDtos
{
    public class EmailConfirmationDto
    {
        public string? Email { get; set; }
        public string? ConfirmationCode { get; set; }
    }
}
