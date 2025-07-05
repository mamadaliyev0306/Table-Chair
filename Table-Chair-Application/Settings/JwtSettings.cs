using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Settings
{
    namespace Table_Chair_Application.Settings
    {
        public class JwtSettings
        {
            public string Issuer { get; set; } = string.Empty;

            public string Secret { get; set; }=string.Empty;
            public int AccessTokenExpirationMinutes { get; set; }
            public int RefreshTokenExpirationDays { get; set; }
            public int EmailVerificationTokenExpirationHours { get; set; }
            public int PasswordResetTokenExpirationMinutes { get; set; }
            public string Audience { get; set; } = string.Empty;
        }
    }
}
