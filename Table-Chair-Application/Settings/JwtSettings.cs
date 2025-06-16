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
            internal string Issuer=string.Empty;

            public string Secret { get; set; }=string.Empty;
            public int AccessTokenExpirationMinutes { get; set; }
            public int RefreshTokenExpirationDays { get; set; }
            public int EmailVerificationTokenExpirationHours { get; set; }
            public int PasswordResetTokenExpirationHours { get; set; }
            public string Audience { get; internal set; } = string.Empty;
        }
    }
}
