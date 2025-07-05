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
    [Table("Faq",Schema ="Models")]
    public class Faq:ISoftDeletable
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; }
        [Required] public string Answer { get; set; } = null!;
        [Required, MaxLength(300)]
        public string Title { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}
