using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Sofdelet;

namespace Table_Chair_Entity.Models
{
    [Table("User", Schema = "Models")]
    public class User:ISoftDeletable
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;  // login uchun (unique)
        [Required]
        [MaxLength(200)]
        public string FirstName { get; set; } = default!;
        [MaxLength(200)]
        public string? LastName { get; set; } 

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string PasswordHash { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public string? Bio {  get; set; } 
        public Role? Role { get; set; } 
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public bool EmailVerified { get; set; } = false;
        public string? PhoneVerificationToken { get; set; }
        public DateTime? PhoneVerificationTokenExpires { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpires { get; set; }
        // Email tasdiqlash uchun token
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpires { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }= new List<RefreshToken>();
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new HashSet<WishlistItem>();
        public virtual ICollection<Cart> Carts { get; set; }=new HashSet<Cart>();
        public virtual ICollection<Product> Products { get; set; }=new List<Product>();
    }

}
