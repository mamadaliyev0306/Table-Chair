using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Responses
{
    public class ValidationErrorResponse : ErrorResponse
    {
        public ValidationErrorResponse(List<string> errors)
            : base("Validation failed", errors) { }
    }
}
