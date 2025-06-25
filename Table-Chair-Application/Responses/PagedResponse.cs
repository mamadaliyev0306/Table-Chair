using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Responses
{
    public class PagedResponse<T> : ApiResponse<List<T>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public static PagedResponse<T> Create(List<T> data, int pageNumber, int pageSize, int totalRecords, string message = "Success")
        {
            return new PagedResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }
    }


}
