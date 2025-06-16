using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Entity.Models
{
    [Table("CartItem",Schema ="Models")]
    public class CartItem
    {
        [Key]
        [Required]
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        [Required]
        public Product Product { get; set; } = null!;
        public int CartId { get; set; }
        public Cart? Cart { get; set; } 
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
