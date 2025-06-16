using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Dtos.AdditionDtos
{
    public class ProductFilterDto
    {
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SearchQuery { get; set; }
        public string? SortBy { get; set; } // "price" yoki "name"
        public bool IsAscending { get; set; } = true;
        public int? MinStockQuantity { get; set; } // Filter by stock
    }

}
