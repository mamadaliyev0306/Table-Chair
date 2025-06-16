using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Exceptions
{
    public class PaymentOperationException : Exception
    {
        public PaymentOperationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
