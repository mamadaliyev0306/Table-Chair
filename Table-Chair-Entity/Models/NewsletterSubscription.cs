using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Entity.Models
{
    [Table("NewsletterSubscription", Schema = "Models")]
    public class NewsletterSubscription
    {
        [Key]
        public int Id { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        public DateTime SubscribedAt { get; set; }= DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
