using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Entity.Enums
{
    public enum PaymentStatus
    {
        Pending,    // To'lov jarayonda
        Completed,  // Muvaffaqiyatli
        Failed,     // To'lov amalga oshmadi
        Refunded,   // Pul qaytarilgan
        PartiallyRefunded, // Qisman qaytarilgan
        Cancelled   // Bekor qilingan
    }
}
