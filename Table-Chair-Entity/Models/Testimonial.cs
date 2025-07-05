using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Migrations;
using Table_Chair_Entity.Sofdelet;

namespace Table_Chair_Entity.Models
{
    [Table("Testimonial",Schema ="Models")]
    public class Testimonial:ISoftDeletable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string AuthorName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }=false;
        public DateTime? DeletedAt {  get; set; }
    }

}
