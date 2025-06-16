using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface ICartService
    {
        Task<List<CartItemDto>> GetCartItemsAsync(int cartId);
        Task<CartDto?> GetCartByUserIdAsync(int userId);
        Task UpdateCartItemQuantityAsync(int cartItemId, int newQuantity);
        Task AddItemToCartAsync(int cartId, CartItemCreateDto item);
        Task RemoveItemFromCartAsync(int cartItemId);
        Task ClearCartAsync(int cartId);
        Task<CartDto> CreateCartForUserAsync(int userId);
        Task<bool> CartExistsAsync(int userId);
        Task<IEnumerable<CartDto>> GetUserCartsAsync(int userId);
        Task<decimal> CalculateCartTotalAsync(int cartId);
        Task<int> GetCartItemCountAsync(int cartId);
    }
}
