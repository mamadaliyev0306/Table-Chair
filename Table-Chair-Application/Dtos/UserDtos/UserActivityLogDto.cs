using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.UserDtos
{
    public class UserActivityLogDto
    {
        public DateTime ActivityDate { get; set; }
        public string? ActivityType { get; set; }
        public string? Description { get; set; }
        public string? IpAddress { get; set; }
    }

}
