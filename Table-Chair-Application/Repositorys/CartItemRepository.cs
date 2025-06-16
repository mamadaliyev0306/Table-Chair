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
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        private readonly FurnitureDbContext _context;
        public CartItemRepository(FurnitureDbContext context) : base(context)
        {
            _context = context;
        }

        public void DeleteRangeAsync(IEnumerable<CartItem> items)
        {
            if (items == null || !items.Any())
                return ;

            _context.CartItems.RemoveRange(items);
        }

        public async Task<CartItem> GetByIdCartItemAsync(int userId, int productId)
        {
            var result = await _context.CartItems.FirstOrDefaultAsync(a=>a.ProductId==productId && a.ProductId==productId);
            if (result == null)
                return new CartItem();
            return result;
        }

    }
}
