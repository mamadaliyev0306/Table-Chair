using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.UserDtos
{
    public class UserProfileUpdateDto
    {
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
    }
}
