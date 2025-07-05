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
    [Table("ContactMessage",Schema ="Models")]
    public class ContactMessage:ISoftDeletable
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string FirstName { get; set; } = null!;
        [Required, MaxLength(200)]
        public string? LastName { get; set; } 

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        public string? Message { get; set; }

        public DateTime SentAt { get; set; }

        public bool IsDeleted { get; set; } 

        public bool IsRead { get; set; } 

        public bool IsResponded { get; set; } 

        public string? PhoneNumber { get; set; } 

        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

}
