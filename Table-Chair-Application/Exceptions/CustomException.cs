using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Exceptions
{
        public abstract class CustomException : Exception
        {
            public int StatusCode { get; }

            protected CustomException(string message, int statusCode) : base(message)
            {
                StatusCode = statusCode;
            }
        }
    

}
