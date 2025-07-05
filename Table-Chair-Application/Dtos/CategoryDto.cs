using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public CategoryType Type { get; set; }
        public bool IsActive { get; set; }
        // Soft delete uchun maydon
        public bool IsDeleted { get; set; }
    }
}
