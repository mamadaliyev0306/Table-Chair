using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly FurnitureDbContext _context;

        public ProductRepository(FurnitureDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId) =>
            await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

        public IQueryable<Product> GetFilteredProductsQuery(ProductFilterDto filter)
        {
            var query = _context.Products.AsQueryable();

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice);

            if (!string.IsNullOrEmpty(filter.SearchQuery))
                query = query.Where(p => p.Name != null && p.Name.Contains(filter.SearchQuery)); // Added null check for p.Name

            query = filter.SortBy?.ToLower() switch
            {
                "price" => filter.IsAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                "name" => filter.IsAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                _ => query.OrderBy(p => p.Id)
            };

            return  query.AsQueryable();
        }

        public IQueryable<Product> SearchAsync(string searchTerm) =>
             _context.Products
                .Where(p => (p.Name != null && p.Name.Contains(searchTerm)) ||
                            (p.Description != null && p.Description.Contains(searchTerm)))
                .AsQueryable();

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.StockQuantity -= quantity;
                _context.Products.Update(product);
            }
        }
        public async Task<PaginatedList<Product>> GetFilteredProductsAsync(ProductFilterDto filterDto, int pageNumber, int pageSize)
        {
            Expression<Func<Product, bool>>? filter = p =>
                (!filterDto.CategoryId.HasValue || p.CategoryId == filterDto.CategoryId) &&
                (!filterDto.MinPrice.HasValue || p.Price >= filterDto.MinPrice) &&
                (!filterDto.MaxPrice.HasValue || p.Price <= filterDto.MaxPrice) &&
                (!filterDto.MinStockQuantity.HasValue || p.StockQuantity >= filterDto.MinStockQuantity) &&
                (string.IsNullOrEmpty(filterDto.SearchQuery) || (p.Name != null && p.Name.Contains(filterDto.SearchQuery)));

            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = filterDto.SortBy?.ToLower() switch
            {
                "price" => filterDto.IsAscending
                    ? products => products.OrderBy(p => p.Price)
                    : products => products.OrderByDescending(p => p.Price),

                "name" => filterDto.IsAscending
                    ? products => products.OrderBy(p => p.Name)
                    : products => products.OrderByDescending(p => p.Name),

                _ => products => products.OrderBy(p => p.Id)
            };

            return await GetFilteredSortedPagedAsync(filter, orderBy, pageNumber, pageSize);
        }

    }
}
