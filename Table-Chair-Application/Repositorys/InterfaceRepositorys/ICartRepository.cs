using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys.InterfaceRepositorys
{
    public interface ICartRepository:IGenericRepository<Cart>
    {
        Task<Cart?> GetByUserIdAsync(int? userId);
        Task UpdateItemQuantityAsync(int cartItemId, int newQuantity);
        Task AddItemAsync(int cartId, CartItem item);
        Task RemoveItemAsync(int cartItemId);
        Task ClearCartAsync(int cartId);
        Task<CartItem?> GetItemByIdAsync(int cartItemId);
        Task LoadItemsWithProductsAsync(int cartId);
        Task<Cart?> GetByIdWithItemsAsync(int id);
    }
}
