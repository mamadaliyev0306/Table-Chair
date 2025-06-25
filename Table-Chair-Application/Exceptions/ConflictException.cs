using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Exceptions
{
    public class ConflictException : AppException
    {
        public ConflictException(string message) : base(message) { }
    }

}
