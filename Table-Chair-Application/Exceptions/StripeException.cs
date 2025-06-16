using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Exceptions
{
    public  class StripeException:Exception
    {
        public StripeException(string message) : base(message)
        {

        }
    }
}
