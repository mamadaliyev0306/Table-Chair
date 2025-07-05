using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.AboutInfoDtos
{
    public class AboutInfoDto
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
