using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Sofdelet;

namespace Table_Chair_Entity.Models
{
    [Table("Blog",Schema ="Models")]
    public class Blog: ISoftDeletable
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; } 

        [Required] public string? Title { get; set; }
        public string? ImageUrl { get; set; }
        [Required] public string? Content { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
