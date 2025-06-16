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
    public interface IWishlistService
    {
        Task<int> GetWishlistCountAsync(int userId);
        Task AddAsync(int userId,WishlistItemCreateDto item);
        Task RemoveAsync(int userId, int productId);
        Task<bool> ExistsAsync(int userId, int productId);
        Task<IEnumerable<WishlistItemDto>> GetWishlistProductsAsync(int userId);
        Task<WishlistToggleResultDto> ToggleWishlistAsync(int userId, int productId);
    }
}
