using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Entity.Models
{
    public class EmailVerification
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string VerificationCode { get; set; }= null!;
        public DateTime ExpiryDate { get; set; }
    }
}
