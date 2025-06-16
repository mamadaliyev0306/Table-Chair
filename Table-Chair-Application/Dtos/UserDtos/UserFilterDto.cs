using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos.UserDtos
{
    public class UserFilterDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        public Role? RoleFilter { get; set; }
        public bool? IsActiveFilter { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sahifa soni 1 dan katta bo'lishi kerak")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Sahifa hajmi 1-100 oralig'ida bo'lishi kerak")]
        public int PageSize { get; set; } = 10;
    }
}
