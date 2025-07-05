using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.UserDtos
{
    public class UserCountStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int DeletedUsers { get; set; }
        public Dictionary<Role, int> UsersByRole { get; set; } = new();
    }
}
