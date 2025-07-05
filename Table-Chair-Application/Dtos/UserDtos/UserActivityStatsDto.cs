using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.UserDtos
{
    public class UserActivityStatsDto
    {
        public DateTime Date { get; set; }
        public int NewUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int LoginCount { get; set; }
    }

}
