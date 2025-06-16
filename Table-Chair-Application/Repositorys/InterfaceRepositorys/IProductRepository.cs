using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys.InterfaceRepositorys
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        IQueryable<Product> SearchAsync(string searchTerm);
        IQueryable<Product> GetFilteredProductsQuery(ProductFilterDto filter);
        Task UpdateStockAsync(int productId, int quantity);
        Task<PaginatedList<Product>> GetFilteredProductsAsync(ProductFilterDto filterDto, int pageNumber, int pageSize);
        
        }
}
