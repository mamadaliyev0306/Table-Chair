using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task AddAsync(CreateProductDto product);
        Task UpdateAsync(UpdateProductDto product);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId);
        IQueryable<ProductDto> SearchProduct(string searchTerm);
        IQueryable<ProductDto> GetFilteredProducts(ProductFilterDto filter);
        Task UpdateStockAsync(int productId, int quantity);
        Task<PaginatedList<ProductDto>> GetFilteredProductsAsync(ProductFilterDto filterDto, int pageNumber, int pageSize);
        Task<PaginatedList<ProductDto>> GetProductsByCategoryAsync(int categoryId, int pageNumber, int pageSize);
        Task<List<ProductDto>> GetProductsWithWishlistInfoAsync(int userId);
    }
}
