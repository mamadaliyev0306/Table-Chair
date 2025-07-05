using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.UserDtos
{
    public class AdminUserResponseDto : UserResponseDto
    {
        public bool IsActive { get; set; }
        public bool EmailVerified { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
