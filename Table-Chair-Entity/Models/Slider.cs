using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Entity.Models
{
    [Table("Slider", Schema = "Models")]
    public class Slider
    {
        [Key]
        public int Id { get; set; }
         public string? ImageUrl { get; set; }
        [MaxLength(150)] public string? Title { get; set; } = string.Empty;
        [MaxLength(300)] public string? Description { get; set; } = string.Empty;
    }
}
