using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos
{
    public class PaginatedList<T>
    {
        public int TotalCount { get; set; } // Barcha mahsulotlar soni
        public int TotalPages => (int)Math.Ceiling((decimal)TotalCount / PageSize); // Barcha sahifalar soni
        public int PageNumber { get; set; } // Joriy sahifa
        public int PageSize { get; set; } // Sahifadagi elementlar soni
        public IEnumerable<T> Items { get; set; } = null!; // Mahsulotlar ro‘yxati
    }
}
