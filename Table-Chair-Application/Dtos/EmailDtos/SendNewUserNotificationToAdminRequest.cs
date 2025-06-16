using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.EmailDtos
{
    public class SendNewUserNotificationToAdminRequest
    {
        [Required, EmailAddress]
        public string AdminEmail { get; set; } = null!;

        [Required, EmailAddress]
        public string NewUserEmail { get; set; } = null!;
    }
}
