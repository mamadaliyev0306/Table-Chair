using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly FurnitureDbContext _context;
        public WishlistRepository(FurnitureDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(WishlistItem item)=>
            await _context.AddAsync(item);

        public async Task<int> CountAsync(int userId)
        {
            var reslut = await _context.WishlistItems.CountAsync();
            return reslut;
        }

        public async Task<bool> ExistsAsync(int userId, int productId)
        {
            return await _context.WishlistItems
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }
        public async Task<IEnumerable<WishlistItem>> GetWishlistProductsAsync(int userId)
        {
            return await _context.WishlistItems
                .Include(a => a.Product)
                 .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task RemoveAsync(int userId, int productId)
        {
            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
            if (item != null)
            {
                _context.WishlistItems.Remove(item);
            }
        }

        public async Task<WishlistItem?> GetWishlistItemAsync(int userId, int productId)
        {
            return await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }
    }
}
