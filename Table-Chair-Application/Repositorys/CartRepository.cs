using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly FurnitureDbContext _context;
        public CartRepository(FurnitureDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(int? userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task UpdateItemQuantityAsync(int cartItemId, int newQuantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = newQuantity;
                cartItem.Cart.UpdatedAt = DateTime.UtcNow;
                _context.CartItems.Update(cartItem);
            }
        }

        public async Task AddItemAsync(int cartId, CartItem item)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart != null)
            {
                cart.Items.Add(item);
                cart.UpdatedAt = DateTime.UtcNow;
                _context.Carts.Update(cart);
            }
        }

        public async Task RemoveItemAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart) // Ensure the Cart is included to access its properties
                .FirstOrDefaultAsync(ci => ci.CartId == cartItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);

                // Update cart's modified date
                if (cartItem.Cart != null) // Check if the Cart is not null
                {
                    cartItem.Cart.UpdatedAt = DateTime.UtcNow;
                    _context.Carts.Update(cartItem.Cart);
                }
            }
        }

        public async Task ClearCartAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.Items);
                cart.UpdatedAt = DateTime.UtcNow;
                _context.Carts.Update(cart);
            }
        }

        public async Task<CartItem?> GetItemByIdAsync(int cartItemId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(a => a.CartItemId == cartItemId);
        }
        public async Task LoadItemsWithProductsAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)  // CartItem ichidagi Product-ga eager loading
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
            {
                throw new NotFoundException($"Cart with ID {cartId} not found");
            }
        }

        public async Task<Cart?> GetByIdWithItemsAsync(int id)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

    }
}
