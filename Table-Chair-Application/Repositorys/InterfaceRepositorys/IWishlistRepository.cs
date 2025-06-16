using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys.InterfaceRepositorys
{
    public interface IWishlistRepository
    {
        Task AddAsync(WishlistItem item);
        Task RemoveAsync(int userId, int productId);
        Task<bool> ExistsAsync(int userId, int productId);
        Task<IEnumerable<WishlistItem>> GetWishlistProductsAsync(int userId);
        Task<int> CountAsync(int userId);
        Task<WishlistItem?> GetWishlistItemAsync(int userId, int productId);
    }
}
