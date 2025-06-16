using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly FurnitureDbContext _context;
        public CategoryRepository(FurnitureDbContext context) : base(context)
        {
            _context = context;
        }
        public IQueryable<Category> GetWithProducts() =>
             _context.Categories
            .Include(c => c.Products)
            .AsQueryable();


        public async Task<bool> HasProductsAsync(int categoryId) =>
            await _context.Products
                .AnyAsync(p => p.CategoryId == categoryId);
        public async Task<IEnumerable<Category>> GetWithProductsAsync()
        {
            return await GetQueryable(includeProperties: "Products").ToListAsync();
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }
        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }
        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories.Where(c => c.IsActive).ToListAsync();
        }

    }
}
