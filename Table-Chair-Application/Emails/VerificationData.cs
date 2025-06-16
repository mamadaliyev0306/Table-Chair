using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Emails
{
    public class VerificationData
    {
        public User User { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
