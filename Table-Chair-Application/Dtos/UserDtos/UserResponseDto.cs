using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.UserDtos
{
    public class UserResponseDto
    {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string FirstName { get; set; } = default!;
            public string? LastName { get; set; } 
            public string PhoneNumber { get; set; } = string.Empty;
            public string Email { get; set; } = default!;
            public string? AvatarUrl { get; set; }
            public string? Bio { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public Role Role { get; set; }
    }
}
