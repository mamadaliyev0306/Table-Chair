using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Entity.Models
{
    [Table("Category",Schema ="Models")]
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(300)]
        public string Name { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
        public virtual ICollection<Faq> Faqs { get; set; } = new List<Faq>();

        public DateTime UpdatedAt { get; set; } 
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }=true;
        // Soft delete uchun maydon
        public bool IsDeleted { get; set; }
    }
}
