using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Responses
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public List<string>? Errors { get; set; }

        public ErrorResponse(string message)
        {
            Message = message;
        }

        public ErrorResponse(string message, List<string> errors)
        {
            Message = message;
            Errors = errors;
        }

        public static ErrorResponse Create(string message)
        {
            return new ErrorResponse(message);
        }

    }
}
