using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys.InterfaceRepositorys
{
    public interface ICategoryRepository: IGenericRepository<Category>
    {
        IQueryable<Category> GetWithProducts();
        Task<bool> HasProductsAsync(int categoryId);
        Task<IEnumerable<Category>> GetWithProductsAsync();
        Task<bool> ExistsAsync(int id);
        Task<Category?> GetByNameAsync(string name);
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    }
}
