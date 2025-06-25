using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> AddAsync(CategoryCreateDto category);
        Task<bool> UpdateAsync(CategoryUpdateDto category);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<CategoryWithProductsDto>> GetWithProductsAsync();
        Task<bool> HasProductsAsync(int categoryId);
        Task<CategoryDto> GetByNameAsync(string name);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
        Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(int categoryId, int pageNumber, int pageSize);
    }
}
