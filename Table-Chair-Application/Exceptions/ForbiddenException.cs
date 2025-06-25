using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "You do not have permission to access this resource.")
            : base(message) { }
    }
}
