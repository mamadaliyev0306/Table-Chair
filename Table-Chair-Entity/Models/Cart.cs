using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Entity.Models
{
    [Table("Carts", Schema = "Models")]
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; } 

        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Umumiy summani hisoblash (ma'lumotlar bazasida saqlanmaydi, faqat hisoblanadi)
        [NotMapped]
        public decimal TotalPrice => Items?.Sum(item => (item.Product?.Price ?? 0) * item.Quantity) ?? 0;

        // Savatdagi jami mahsulotlar soni
        [NotMapped]
        public int TotalItems => Items?.Sum(item => item.Quantity) ?? 0;
    }
}
