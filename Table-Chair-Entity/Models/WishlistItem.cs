using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Entity.Models
{
    [Table("WishlistItem",Schema ="Models")]
        public class WishlistItem
        {
            [Key]
            public int Id { get; set; } 

            public int UserId { get; set; }
           [ForeignKey(nameof(UserId))]
            public virtual User? User { get; set; } 

            public int ProductId { get; set; }
            [ForeignKey(nameof(ProductId))]
            public virtual Product Product { get; set; } = null!;

            public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        }

    
}
